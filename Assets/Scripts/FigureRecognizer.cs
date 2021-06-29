using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;

public class FigureRecognizer
{
    private readonly FigureDatabase[] _databases;

    private readonly IWorker[] _workers;

    private readonly float[] _buffer = new float[32 * 32];

    private readonly Tensor _input = new Tensor(n: 1, c: 32 * 32);

    public FigureRecognizer(IEnumerable<FigureDatabase> databases)
    {
        if (databases == null)
        {
            throw new ArgumentNullException(nameof(databases));
        }

        _databases = databases.ToArray();
        for (int i = 0; i < _databases.Length; i++)
        {
            if (_databases[i] == null)
            {
                throw new NullReferenceException($"{nameof(databases)}. Index - {i}");
            }
        }

        NNModel[] neuralNetworks = _databases.Select(database => database.LoadNeuralNetwork()).ToArray();
        _workers = new IWorker[neuralNetworks.Length];
        for (int i = 0; i < neuralNetworks.Length; i++)
        {
            if (neuralNetworks[i] == null)
            {
                throw new NullReferenceException($"{nameof(databases)}. Neural network for database with index - {i}");
            }

            Model model = ModelLoader.Load(neuralNetworks[i]);
            _workers[i] = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpRef, model);
        }
    }

    public Figure Recognize(BitBuffer figureBuffer)
    {
        figureBuffer.CopyTo(_buffer);
        for (int i = 0; i < _buffer.Length; i++)
        {
            _input[i] = _buffer[i];
        }

        for (int databaseIndex = 0; databaseIndex < _databases.Length; databaseIndex++)
        {
            IWorker worker = _workers[databaseIndex];
            Tensor output = worker.Execute(_input).PeekOutput();
            float[] result = output.AsFloats();
            for (int figureIndex = 0; figureIndex < result.Length; figureIndex++)
            {
                if (result[figureIndex] > 0.75f)
                {
                    return _databases[databaseIndex].GetFigure(figureIndex);
                }
            }
        }

        return null;
    }

    public void Destroy()
    {
        _input.Dispose();
        foreach (IWorker worker in _workers)
        {
            worker.Dispose();
        }
    }
}
