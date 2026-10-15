"""Explicit, unreleased tower overlay on the merged 155-source candidate."""
from pathlib import Path
import hashlib, json, os, re, shutil, subprocess, sys, xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[2]
here = Path(__file__).parent
out = root / 'build/tower-integration-research'
src = out / 'source'
manifest = json.loads((here / 'patch-manifest.json').read_text())
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()

def apply_patch(text, patch):
    """Apply only exact unified hunks; never search/fuzz around changed context."""
    original = text.splitlines(True)
    lines = patch.splitlines(True)
    result, cursor, i = [], 0, 2
    while i < len(lines):
        match = re.fullmatch(r'@@ -(\d+)(?:,(\d+))? \+(\d+)(?:,(\d+))? @@\n', lines[i])
        assert match, 'Invalid patch header'
        old_count = int(match[2] or 1)
        new_count = int(match[4] or 1)
        start = int(match[1]) - (1 if old_count else 0)
        old, new = [], []
        i += 1
        while i < len(lines) and not lines[i].startswith('@@ '):
            line = lines[i]
            assert line[:1] in (' ', '-', '+'), 'Invalid hunk line'
            if line[0] != '+': old.append(line[1:])
            if line[0] != '-': new.append(line[1:])
            i += 1
        assert len(old) == old_count and len(new) == new_count
        assert cursor <= start and original[start:start + old_count] == old, 'Patch context changed'
        result.extend(original[cursor:start])
        result.extend(new)
        cursor = start + old_count
    result.extend(original[cursor:])
    return ''.join(result)

def build():
    subprocess.run([sys.executable, str(root / 'research/integration/build.py')], check=True, cwd=root)
    base = root / 'build/integration-research'
    src.mkdir(parents=True, exist_ok=True)
    base_sources = sorted((base / 'source').glob('*.cs'))
    assert len(base_sources) == 155
    for p in base_sources: shutil.copyfile(p, src / p.name)
    for name, expected in manifest['inputSources'].items():
        p = src / name
        assert sha(p) == expected, 'Review updated shared input: ' + name
        text = apply_patch(p.read_text('utf-8-sig'), (here / 'patches' / (name + '.patch')).read_text('utf-8'))
        p.write_text(text, encoding='utf-8-sig', newline='')
        assert sha(p) == manifest['outputSources'][name], name
    tower = root / 'research/tower/src/Tools/VmTestKit'
    extras = {p.name for p in tower.glob('*.cs')} - {p.name for p in (root / 'src/Tools/VmTestKit').glob('*.cs')}
    assert extras == {'Death-Tower-Floors-FG171.cs', 'Tower-Drop-Planner-FG175.cs', 'Tower-Experience-FG173.cs',
                      'Tower-Ground-Service-FG175.cs', 'Tower-Rewards-FG174.cs', 'Tower-Service-FG172.cs', 'Typed-Enemy-Drops-FG171.cs'}
    assert extras == set(manifest['towerSourceSha256'])
    for name in extras:
        assert sha(tower / name) == manifest['towerSourceSha256'][name], 'Review tower class ' + name
        shutil.copyfile(tower / name, src / name)
    for name, hashes in manifest.get('towerFollowupPatches', {}).items():
        assert name in extras and sha(src / name) == hashes['input']
        text = apply_patch((src / name).read_text('utf-8-sig'), (here / 'patches' / (name + '.patch')).read_text('utf-8'))
        (src / name).write_text(text, encoding='utf-8-sig', newline='')
        assert sha(src / name) == hashes['output'], 'Tower followup output ' + name
    expected_names = {p.name for p in base_sources} | extras
    assert {p.name for p in src.glob('*.cs')} == expected_names and len(expected_names) == 162
    for p in base.iterdir():
        if p.is_file() and p.suffix in ('.xml', '.dat'): shutil.copyfile(p, out / p.name)
    # Reuse the already versioned derived catalog instead of committing a second
    # copy of 2508 rows. Preserve the current base and reject changed shared IDs.
    current_items = ET.parse(base / 'item-metadata.xml').getroot()
    tower_items = ET.parse(root / 'research/tower/data/item-metadata.xml').getroot()
    assert current_items.findtext('PvfSha256') == tower_items.findtext('PvfSha256')
    existing = {e.findtext('ItemId'): e for e in current_items.find('Items')}
    extra_items = []
    signature = lambda e: tuple((c.tag, c.text or '') for c in e)
    for e in tower_items.find('Items'):
        ident = e.findtext('ItemId')
        if ident in existing: assert signature(e) == signature(existing[ident]), 'Review item ' + ident
        else: extra_items.append(e)
    assert len(extra_items) == manifest['newItemCount']
    document = ET.Element('Offline70ItemMetadataDocumentFG')
    ET.SubElement(document, 'Format').text = '1'
    ET.SubElement(document, 'PvfSha256').text = current_items.findtext('PvfSha256')
    ET.SubElement(document, 'Items').extend(extra_items)
    ET.indent(document, space='  ')
    (out / 'tower-item-metadata-FG184.xml').write_bytes(ET.tostring(document, encoding='utf-8', xml_declaration=True))
    rewards = root / 'research/tower/data/tower-rewards-FG174.dat'
    assert sha(rewards) == manifest['rewardTableSha256']
    shutil.copyfile(rewards, out / 'tower-rewards-FG174.dat')
    quest = out / 'quest-catalog.xml'
    text = quest.read_text('utf-8-sig')
    pattern = re.compile(r'  <Offline70QuestDefinitionFG>\s*<Id>(\d+)</Id>.*?</Offline70QuestDefinitionFG>', re.S)
    overrides = {m[1]: m[0] for m in pattern.finditer((here / 'data/quest-overrides-FG184.xml').read_text('utf-8'))}
    assert set(overrides) == set(manifest['questInputs'])
    seen = set()
    def replace(m):
        ident = m[1]
        if ident not in overrides: return m[0]
        assert hashlib.sha256(m[0].encode()).hexdigest() == manifest['questInputs'][ident], 'Review quest ' + ident
        assert ident not in seen
        seen.add(ident)
        return overrides[ident]
    text = pattern.sub(replace, text)
    assert seen == set(overrides)
    quest.write_text(text, encoding='utf-8-sig', newline='')
    framework = Path(os.environ.get('WINDIR', 'C:/Windows')) / 'Microsoft.NET/Framework'
    compiler = next((p for p in [framework/'v3.5/csc.exe', framework/'v4.0.30319/csc.exe'] if p.is_file()), None)
    assert compiler
    env = {k.upper(): v for k, v in os.environ.items()}
    exe = out / 'DNF70-Channel-FG.exe'
    r = subprocess.run([str(compiler), '/nologo', '/target:exe', '/main:Offline70BatchServiceFG', '/out:' + str(exe),
                        *map(str, sorted(src.glob('*.cs')))], capture_output=True, env=env)
    (out / 'compile.txt').write_bytes(r.stdout + r.stderr)
    assert r.returncode == 0, r.stdout + r.stderr
    r = subprocess.run([str(exe)], capture_output=True, timeout=15, env=env)
    assert r.returncode == 2
    proof = dict(sourceCount=162, serviceSha256=sha(exe), sources={p.name: sha(p) for p in src.glob('*.cs')},
                 data={p.name: sha(p) for p in out.iterdir() if p.suffix in ('.xml', '.dat')},
                 baseMain=manifest['baseMain'], releaseAllowed=False, deployed=False, smokePassed=True)
    (out / 'build-proof.json').write_text(json.dumps(proof, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(dict(compiled=162, serviceSha256=proof['serviceSha256'], releaseAllowed=False)))

if __name__ == '__main__': build()
