from tensorflow import keras
from tensorflow.keras.layers import Dense
from utils.dataset_loader import load_data
from utils.model_saver import save_model

model_name = 'MainFigures'

database_id = '4c200220a2fd24e4e9379ab72801e496'

dataset_path = f'../Datasets/{model_name}.dataset'

id_to_name = [
	"Vertical line",
	"Horizontal line",
	"Phi",
	"Delta",
	"Epsilon"
]

(x_train, y_train), (x_test, y_test) = load_data(dataset_path, database_id, id_to_name)

model = keras.Sequential(
	[
		Dense(units=32, input_shape=(32 * 32,), activation='relu'),
		Dense(units=5, activation='softmax')
	],
	model_name
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

save_model(model, model_name, database_id)
