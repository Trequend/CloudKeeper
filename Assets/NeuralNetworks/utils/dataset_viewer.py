import matplotlib.pyplot as plt
import numpy as np
from utils.dataset_loader import read_dataset

def view_dataset(path, id_to_name):
	dataset = read_dataset(path)
	database_id = dataset['database_id']
	elements = dataset['elements']
	elements_count = len(elements)

	if elements_count == 0:
		print('Dataset is empty')
		return

	def parse_entry(index):
		element = elements[index]
		name = id_to_name[element['id']]
		pattern = element['pattern']
		width = 32
		height = 32
		image_data = np.zeros((height, width, 3), dtype=np.uint8)
		for y in range(height):
			y_offset = y * width
			for x in range(width):
				if pattern[y_offset + x] == '1':
					image_data[y, x] = [255, 255, 255]
		return name, image_data

	dataset_figure, axs = plt.subplots(nrows=3, ncols=3)
	dataset_figure.canvas.manager.set_window_title(f'Dataset ({database_id})')
	
	for _, ax in enumerate(axs.ravel()):
		index = np.random.randint(0, elements_count)
		name, image_data = parse_entry(index)
		ax.get_xaxis().set_visible(False)
		ax.get_yaxis().set_visible(False)
		ax.imshow(image_data, interpolation='nearest')
		ax.set_title(name)
	
	plt.tight_layout()
	plt.show()

