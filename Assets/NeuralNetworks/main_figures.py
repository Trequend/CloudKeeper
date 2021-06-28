from tensorflow import keras
from tensorflow.keras.layers import Dense
import keras2onnx
import numpy as np
import json

dataset_path = '../Datasets/MainFiguresDataset.json'

onnx_model_folder = '../Resources/NeuralNetworks/'

id_to_name = [
	"Vertical line",
	"Horizontal line",
	"Phi",
	"Delta",
	"Epsilon"
]

def load_data(path, test_percent=10):
	with open(path) as file:
		dataset = json.load(file)
		databaseId = dataset["DatabaseId"]
		figures_count = len(id_to_name)
		test_count = [0] * figures_count
		elements = dataset["Elements"]
		elements_count = len(elements)
		test_size = int(elements_count / figures_count * test_percent / 100)
		x_train = []
		y_train = []
		x_test = []
		y_test = []
		for element in elements:
			id = int(element["Id"])
			pattern = element["Pattern"]
			x = []
			y = keras.utils.to_categorical(id, figures_count)
			for letter in pattern:
				x.append(float(letter))
			if (test_count[id] > test_size):
				x_train.append(x)
				y_train.append(y)
			else:
				test_count[id] += 1
				x_test.append(x)
				y_test.append(y)
		x_train = np.asarray(x_train).astype('float32')
		y_train = np.asarray(y_train).astype('float32')
		x_test = np.asarray(x_test).astype('float32')
		y_test = np.asarray(y_test).astype('float32')
		return databaseId, (x_train, y_train), (x_test, y_test)

databaseId, (x_train, y_train), (x_test, y_test) = load_data(dataset_path)

model = keras.Sequential(
	[
		Dense(units=32, input_shape=(32 * 32,), activation='relu'),
		Dense(units=5, activation='softmax')
	],
	'Test'
)

model.compile(
	loss=keras.losses.CategoricalCrossentropy(),
	optimizer=keras.optimizers.Adam(),
	metrics=['accuracy']
)

model.fit(x_train, y_train, batch_size=32, epochs=3, verbose=True)

print()
print("Start test")
model.evaluate(x_test, y_test)
print("End test")
print()

onnx_model = keras2onnx.convert_keras(model, 'MainFigures')
keras2onnx.save_model(onnx_model, f'{onnx_model_folder}{databaseId}.onnx')
