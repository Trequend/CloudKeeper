import matplotlib.pyplot as plt
from tensorflow import keras
from tensorflow.keras.layers import Dense, Conv2D, MaxPooling2D, Reshape, Dropout
from tensorflow.python.keras.layers.core import Dropout, Flatten
from utils.dataset_loader import load_data
from utils.model_saver import save_model

model_name = 'MainFigures'

database_id = '4c200220a2fd24e4e9379ab72801e496'

dataset_path = f'../Datasets/{model_name}.dataset'

validation_dataset_path = f'../Datasets/{model_name}_Validation.dataset'

test_dataset_path = f'../Datasets/{model_name}_Test.dataset'

id_to_name = [
	"Vertical line",
	"Horizontal line",
	"Phi",
	"Caret",
	"Epsilon",
	"Vi",
	"Unknown"
]

print()
print("Start loading dataset")
(x_train, y_train), (x_validation, y_validation), (x_test, y_test) = load_data(
	dataset_path,
	validation_dataset_path,
	test_dataset_path,
	database_id,
	id_to_name
)
print("Dataset loaded")
print()

model = keras.Sequential(
	[
		Reshape((32, 32, 1), input_shape=(32 * 32,)),
		Conv2D(32, (6, 6), activation='relu'),
		MaxPooling2D(pool_size=(2, 2)),
		Conv2D(64, (3, 3), activation='relu'),
		Conv2D(5, (3, 3), activation='relu'),
		MaxPooling2D(pool_size=(2, 2)),
		Flatten(),
		Dropout(0.5),
		Dense(units=7, activation='softmax')
	],
	model_name
)

model.compile(
	loss=keras.losses.CategoricalCrossentropy(),
	optimizer=keras.optimizers.Adam(),
	metrics=['accuracy']
)

history = model.fit(
	x_train,
	y_train,
	batch_size=128,
	validation_data=(x_validation, y_validation),
	epochs=15,
	verbose=True
)

print()
print("Start test")
model.evaluate(x_test, y_test)
print("End test")
print()

save_model(model, model_name, database_id)

plt.plot(history.history['loss'])
plt.plot(history.history['val_loss'])
plt.show()
