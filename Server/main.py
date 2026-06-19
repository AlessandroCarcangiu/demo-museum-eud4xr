from flask import Flask, request
import base64
import os

app = Flask(__name__)

PUBLIC_DATA_FOLDER = "public-data"
os.makedirs(PUBLIC_DATA_FOLDER, exist_ok=True)  # Ensure upload folder exists

@app.route('/get-json-config', methods=['GET'])
def get_json_config():
    # Get json data from public-data/config.json
    config_path = os.path.join(PUBLIC_DATA_FOLDER, 'marker-data-config.json')
    if not os.path.exists(config_path):
        return {"error": "Config file not found"}, 404
    with open(config_path, 'r') as f:
        config = f.read()

    return config


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8090, debug=True, use_reloader=False)
