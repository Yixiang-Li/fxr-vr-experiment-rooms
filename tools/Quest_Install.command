#!/bin/zsh
cd "$(dirname "$0")/.." || exit 1
python3 tools/quest_device.py install
read '?Press Enter to close.'
