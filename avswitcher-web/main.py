#!/usr/bin/python

import yaml
from flask import Flask, render_template, request
import flask
from threading import Thread
from Queue import Queue
import os, sys, signal
from time import sleep
import pickle

app = Flask(__name__)

working_dir = os.path.dirname(os.path.realpath(__file__))
state_path = os.path.join(working_dir, "state.pickle")
current_inputs = {}

class InputSources(object):
  def __init__(self, file_name):
    self.data = yaml.load(open(os.path.join(working_dir, file_name), 'r'))

  def get_remote_for_type(self, input_type):
    for source in self.data['inputSources']:
      if source['key'] == input_type:
        return source['lircRemote']
    raise Exception("Unknown input type: %s" % input_type)

  def contains_index(self, input_type, index):
    for source in self.data['inputSources']:
        if source['key'] == input_type:
          return int(index) in map(lambda i: i['index'], source['items'])
    return False

@app.route('/')
def index():
  return render_template('index.html',
    input_sources=input_sources.data,
    current_inputs=current_inputs,
    static_root="//cloudhacking-assets.s3.amazonaws.com/avswitcher/static")

def submit_lirc_command(code):
  lirc_command_queue.put(code)

@app.route('/switch_input', methods=['POST'])
def switch_input():
  desired_source = request.form['source']
  input_type = request.form['type']
  which_remote = input_sources.get_remote_for_type(input_type)
  if input_sources.contains_index(input_type, desired_source):
    submit_lirc_command('%s KEY_PROG%s' % (which_remote, desired_source))
    current_inputs[input_type] = int(desired_source)
    pickle.dump(current_inputs, open(state_path, "w"))
    return flask.jsonify({'result': 'OK'})
  else:
    return flask.jsonify({'result': 'Bad request'}), 400

def lirc_command_worker(q):
  while True:
    code = q.get()
    if code == 'EXIT':
      break
    ret = os.system('irsend SEND_START %s' % code)
    if ret == 0:
      sleep(0.3)
      os.system('irsend SEND_STOP %s' % code)

# We need to install an interrupt handler so that we can shut down
# the worker thread and exit cleanly.
def interrupt_handler(signal, frame):
  lirc_command_queue.put('EXIT')
  worker_thread.join()
  sys.exit(0)
signal.signal(signal.SIGINT, interrupt_handler)

if __name__ == '__main__': 
  input_sources = InputSources("input-sources.yaml")
  if os.path.exists(state_path):
    try:
      current_inputs = pickle.load(open(state_path, "r"))
    except:
      pass

  lirc_command_queue = Queue()
  worker_thread = Thread(target=lirc_command_worker, args=(lirc_command_queue,))
  worker_thread.start()

  app.debug = True
  app.run(host='0.0.0.0')

