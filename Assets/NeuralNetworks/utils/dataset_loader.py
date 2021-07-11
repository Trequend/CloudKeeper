import numpy as np
from tensorflow import keras

def read_dataset(path):
	with open(path) as file:
		database_id = file.readline().strip()
		elements = []
		for line in file:
			tokens = line.split(' ')
			if len(tokens) != 2:
				raise ValueError('Wrong dataset entry')
			id = int(tokens[0])
			pattern = tokens[1].strip()
			elements.append({
				'id': id,
				'pattern': pattern
			})
		return {
			'database_id': database_id,
			'elements': elements
		}

def parse_dataset(path, figures_count, required_id=None):
	x_dataset = []
	y_dataset = []
	dataset = read_dataset(path)
	database_id = dataset['database_id']
	if required_id != None and database_id != required_id:
		raise ValueError(f'Wrong database id in file \"{path}\"')
	elements = dataset['elements']
	for element in elements:
		id = int(element["id"])
		pattern = element["pattern"]
		x = []
		y = keras.utils.to_categorical(id, figures_count)
		for letter in pattern:
			x.append(float(letter))
		x_dataset.append(x)
		y_dataset.append(y)
	x_dataset = np.asarray(x_dataset).astype('float32')
	y_dataset = np.asarray(y_dataset).astype('float32')
	return (x_dataset, y_dataset)

def load_data(path, validation_path, test_path, database_id, id_to_name):
	figures_count = len(id_to_name)
	dataset = parse_dataset(path, figures_count, database_id)
	validation_dataset = parse_dataset(validation_path, figures_count, database_id)
	test_dataset = parse_dataset(test_path, figures_count, database_id)
	return dataset, validation_dataset, test_dataset
