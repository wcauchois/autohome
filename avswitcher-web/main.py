import json
from flask import Flask, render_template, request
import flask
from threading import Thread
from Queue import Queue
import os, sys, signal
from time import sleep

app = Flask(__name__)

input_sources_list = []

@app.route('/')
def hello():
  return render_template('index.html', inputsources=input_sources_list)

def submit_lirc_command(code):
  lirc_command_queue.put(code)

@app.route('/switch_hdmi', methods=['POST'])
def switch_hdmi():
  desired_source = request.form['source']
  if desired_source in map(str, range(1, 5)):
    submit_lirc_command('KEY_PROG%s' % desired_source)
    return flask.jsonify({'result': 'OK'})
  else:
    return flask.jsonify({'result': 'Bad request'}), 400
    

def lirc_command_worker(q):
  #return
  while True:
    code = q.get()
    if code == 'EXIT':
      break
    ret = os.system('irsend SEND_START my_remote %s' % code)
    if ret == 0:
      sleep(0.3)
      os.system('irsend SEND_STOP my_remote %s' % code)

# We need to install an interrupt handler so that we can shut down
# the worker thread and exit cleanly.
def interrupt_handler(signal, frame):
  lirc_command_queue.put('EXIT')
  worker_thread.join()
  sys.exit(0)
signal.signal(signal.SIGINT, interrupt_handler)

if __name__ == '__main__': 
  input_sources_json = json.load(open('inputsources.json', 'r'))
  input_sources_list = input_sources_json.keys()

  lirc_command_queue = Queue()
  worker_thread = Thread(target=lirc_command_worker, args=(lirc_command_queue,))
  worker_thread.start()

  app.run(host='0.0.0.0')

