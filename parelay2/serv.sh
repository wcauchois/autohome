#!/bin/bash

PORT=3100
DEVICE="hw:1,0"

while true; do
  nc -l -p $PORT | zcat -d | aplay -D "$DEVICE" -f cd
done
