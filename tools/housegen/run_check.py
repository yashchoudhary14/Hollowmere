"""Validate every house and the village layout. Exit code 1 if anything fails.
Usage (from the repo root):  python tools/housegen/run_check.py [houses_a houses_b houses_c] [-v]"""
import sys, importlib
from core import validate
from village import build

args = [a for a in sys.argv[1:] if not a.startswith("-")]
verbose = "-v" in sys.argv
mods = args or ["houses_a", "houses_b", "houses_c"]
errors = 0
for mn in mods:
    m = importlib.import_module(mn)
    for fn in sorted(n for n in dir(m) if n.startswith("h") and n[1:].isdigit()):
        h = getattr(m, fn)()
        e, w = validate(h)
        errors += len(e)
        print(f"== {h.slug} {h.name}: {len(h.rooms)} rooms, {len(h.props)} props, {len(e)} errors, {len(w)} warnings")
        for x in e: print("   E", x)
        if verbose:
            for x in w: print("   w", x)
problems = build()[1]["problems"]
print(f"village problems: {len(problems)}")
for p in problems: print("   V", p)
sys.exit(1 if errors or problems else 0)
