#!/bin/bash
echo "Sources:"
pacmd list-sources | grep "name:"
echo
echo "Sinks:"
pacmd list-sinks | grep "name:"

