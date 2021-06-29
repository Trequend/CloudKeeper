import keras2onnx

onnx_model_folder = '../Resources/NeuralNetworks/'

def save_model(model, name, database_id):
	onnx_model = keras2onnx.convert_keras(model, name)
	keras2onnx.save_model(onnx_model, f'{onnx_model_folder}{database_id}.onnx')