import openai
import tensorflow as tf
import numpy as np
from flask import Flask, request, jsonify
import io
from PIL import Image
from collections import deque

app = Flask(__name__)

# OpenAI API 설정
openai.api_key = "write/your/openai/api_key"  # OpenAI API Key를 입력하세요.

# TensorFlow Lite 모델 로드 및 초기화
model_path = "import/your/tflite/file"
model = tf.lite.Interpreter(model_path=model_path)
model.allocate_tensors()
input_details = model.get_input_details()
output_details = model.get_output_details()

# Confidence 값 스무딩을 위한 큐
confidence_history = deque(maxlen=5)  # 최근 5개의 confidence 값 저장


def predict_drowsiness(image_data):
    try:
        # 이미지를 열고 전처리
        image = Image.open(io.BytesIO(image_data)).convert("RGB")
        print(f"Original Image Size: {image.size}")  # 원본 이미지 크기 출력
        image = image.resize((64, 64))  # 모델이 요구하는 크기로 리사이즈
        print(f"Resized Image Size: {image.size}")  # 리사이즈 후 이미지 크기 출력

        # 입력 데이터를 float32로 변환하고 정규화
        input_data = np.array(image, dtype=np.float32) / 255.0
        input_data = input_data.reshape(input_details[0]['shape'])

        # 모델에 입력 데이터 설정
        model.set_tensor(input_details[0]['index'], input_data)
        model.invoke()

        # 모델 출력 가져오기
        output_data = model.get_tensor(output_details[0]['index'])

        # Confidence 스무딩
        current_confidence = float(output_data[0][0])
        confidence_history.append(current_confidence)
        smoothed_confidence = sum(confidence_history) / len(confidence_history)

        # 졸음 여부 판단 (임계값 0.5로 설정)
        is_drowsy = smoothed_confidence > 0.5
        return is_drowsy
    except Exception as e:
        print(f"Error processing image: {e}")
        return False


@app.route('/drowsiness', methods=['POST'])
def drowsiness():
    if 'image' not in request.files:
        return jsonify({'message': 'No image provided'}), 400

    image_file = request.files['image']
    image_data = image_file.read()

    # 모델을 사용해 졸음 상태 예측
    is_drowsy = predict_drowsiness(image_data)

    # 콘솔에 drowsiness 상태 출력
    print(f"drowsiness: {is_drowsy}")

    # 졸음 상태에 따른 메시지 생성
    if is_drowsy:
        response = openai.ChatCompletion.create(
            model="gpt-3.5-turbo",
            messages=[{"role": "system", "content": "User is drowsy. Wake them up with a friendly message!"}]
        )
        message = response.choices[0].message['content']
        return jsonify({'drowsiness': True, 'message': message}), 200
    else:
        return jsonify({'drowsiness': False, 'message': 'User is alert!'}), 200


if __name__ == "__main__":
    app.run(host='0.0.0.0', port=5000, debug=True)
