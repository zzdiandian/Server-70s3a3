"""Compiled boundaries only; no listener, original client, or player data."""
from pathlib import Path
import json, os, subprocess, uuid

root=Path(__file__).resolve().parents[2]
here=Path(__file__).parent
out=root/'build/tower-integration-research'
proof=json.loads((out/'build-proof.json').read_text())
framework=Path(os.environ.get('WINDIR','C:/Windows'))/'Microsoft.NET/Framework'
compiler=next(p for p in [framework/'v3.5/csc.exe',framework/'v4.0.30319/csc.exe'] if p.is_file())
env={k.upper():v for k,v in os.environ.items()}
for name,fixture in [('TowerRebaseBoundariesFG184',here/'tests/Boundaries-FG184.cs'),
                     ('TowerRetryBoundariesFG185',here/'tests/Retry-Boundaries-FG185.cs'),
                     ('IntegrationBoundariesFG183',root/'research/integration/tests/Boundaries-FG183.cs')]:
    exe=out/(name+'.exe')
    r=subprocess.run([str(compiler),'/nologo','/target:exe','/main:'+name,'/out:'+str(exe),
                      *map(str,sorted((out/'source').glob('*.cs'))),str(fixture)],capture_output=True,env=env)
    (out/(name+'-compile.txt')).write_bytes(r.stdout+r.stderr)
    assert r.returncode==0,r.stdout+r.stderr
    r=subprocess.run([str(exe),str(out)],capture_output=True,timeout=30,env=env)
    (out/(name+'-result.txt')).write_bytes(r.stdout+r.stderr)
    assert r.returncode==0,r.stdout+r.stderr
    report=json.loads(r.stdout);report['serviceSha256']=proof['serviceSha256']
    (out/(name+'-result.json')).write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report))

# Run the existing economic regressions against these 162 sources. Their own
# guards accept only their original build fixture roots, so keep that location;
# the executable and data arguments still point to this tower candidate.
for name,fixture,scope in [
    ('ReinforcementTransactionsFG176','research/reinforcement/tests/Transactions-FG176.cs','reinforcement-research'),
    ('BreakTransactionsFG177','research/reinforcement/tests/Break-Transactions-FG177.cs','reinforcement-research'),
    ('RetryProjectionTestsFG178','research/reinforcement/tests/Retry-Projection-FG178.cs','reinforcement-research'),
    ('ProtectionTransactionsFG180','research/reinforcement/tests/Protection-Transactions-FG180.cs','reinforcement-research'),
    ('WorldTransactions182','research/worlddrop/tests/World-Transactions-FG182.cs','worlddrop-research')]:
    trial=root/'build'/scope/('tower-184-'+uuid.uuid4().hex[:8]);trial.mkdir(parents=True)
    exe=out/(name+'.exe')
    r=subprocess.run([str(compiler),'/nologo','/target:exe','/main:'+name,'/out:'+str(exe),
                      *map(str,sorted((out/'source').glob('*.cs'))),str(root/fixture)],capture_output=True,env=env)
    (trial/'compile.txt').write_bytes(r.stdout+r.stderr)
    assert r.returncode==0,r.stdout+r.stderr
    args=[str(exe),str(trial),str(out)]
    if name=='WorldTransactions182': args.append(str(root/'research/worlddrop/tests/native-quotes.tsv'))
    r=subprocess.run(args,capture_output=True,timeout=120,env=env)
    (trial/'result.txt').write_bytes(r.stdout+r.stderr)
    assert r.returncode==0,r.stdout+r.stderr
    report=json.loads(r.stdout);report['serviceSha256']=proof['serviceSha256']
    (trial/'result.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    (out/(name+'-result.json')).write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report))
