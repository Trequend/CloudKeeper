import numpy as np
from tensorflow import keras

def read_dataset(path):
	with open(path) as file:
		database_id = file.readline()
		elements = []
		for line in file:
			tokens = line.split(' ')
			if len(tokens) != 2:
				raise ValueError('Wrong dataset entry')
			id = int(tokens[0])
			pattern = tokens[1]
			elements.append({
				'id': id,
				'pattern': pattern
			})
		return {
			'database_id': database_id,
			'elements': elements
		}

def load_data(path, database_id, id_to_name, test_percent=10):
	dataset = read_dataset(path)
	if dataset['database_id'] != database_id:
		raise ValueError('Wrong database id')
	figures_count = len(id_to_name)
	test_count = [0] * figures_count
	elements = dataset['elements']
	elements_count = len(elements)
	test_size = int(elements_count / figures_count * test_percent / 100)
	x_train = []
	y_train = []
	x_test = []
	y_test = []
	for element in elements:
		id = int(element["id"])
		pattern = element["pattern"]
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
	return (x_train, y_train), (x_test, y_test)