using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class FigureRecognizer
{
    private readonly FigureDatabase[] _databases;

    private readonly IWorker[] _workers;

    private readonly float[] _buffer = new float[32 * 32];

    private readonly Tensor _input = new Tensor(n: 1, c: 32 * 32);

    private float _threshold;

    public float Threshold
    {
        get => _threshold;
        set => _threshold = Mathf.Clamp(value, 0.01f, 0.99f);
    }

    public FigureRecognizer(IEnumerable<FigureDatabase> databases, float threshold = 0.75f)
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

        Threshold = _threshold;
    }

    public Figure Recognize(BitBuffer figureBuffer)
    {
        return Recognize(figureBuffer, out _);
    }

    public Figure Recognize(BitBuffer figureBuffer, out int index)
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
                if (result[figureIndex] > _threshold)
                {
                    index = figureIndex;
                    return _databases[databaseIndex].GetFigure(figureIndex);
                }
            }
        }

        index = -1;
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
