import json
from flask import Flask, render_template

app = Flask(__name__)

input_sources_list = []

@app.route("/")
def hello():
  return render_template('index.html', inputsources=input_sources_list)

if __name__ == '__main__': 
  input_sources_json = json.load(open('inputsources.json', 'r'))
  input_sources_list = input_sources_json.keys()
  app.run(host='0.0.0.0')
