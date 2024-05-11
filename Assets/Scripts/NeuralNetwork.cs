using UnityEngine;

public class NeuralNetwork
{
    public int[] network = new int[] {2, 4, 4, 2 }; //modifica pure come vuoi, il primo valore è il numero di input, l'ultimo di output
    public Layer[] layers;

    public NeuralNetwork(int[] network)
    {
        this.network = network;
        Awake();
    }

    public class Layer{
        public float[,] weights; // weights[i,j] = nodo i, peso j
        public float[] biases; 
        public float[] nodes; 

        private int n_inputs;
        public int n_nodes{ get; private set;}


        public Layer(int n_inputs, int n_nodes)
        {
            this.n_inputs = n_inputs;
            this.n_nodes = n_nodes;

            weights = new float[n_nodes, n_inputs];
            biases = new float[n_nodes];
            nodes = new float[n_nodes];
        }

        public void Forward(float[] inputs) //calcolo formula y = sommatoria(input * peso) + bias
        {
            nodes = new float[n_nodes];

            for (int i = 0; i < n_nodes; i++)
            {
                // sommatoria pesi * input
                for (int j = 0; j < n_inputs; j++)
                {
                    nodes[i] += inputs[j] * weights[i, j];
                }
                // somma + bias
                nodes[i] += biases[i];
            }
        }

        public void Activation() // utilizza la funzione ReLU, non so se sia la migliore per questa rete, in caso utilizziamo la sigmoide
        {
            for (int i = 0; i < n_nodes; i++)
            {
                nodes[i] = Mathf.Max(0f, nodes[i]);
            }

            // sigmoide
            // for (int i = 0; i < n_nodes; i++)
            // {
            //     nodes[i] = 1f / (1f + Mathf.Exp(-nodes[i]));
            // }
        }
        public void Mutate(float mutationAmount, float MutationChance) //mutation chance è per non far mutare tutti i layer
        {
            for (int i = 0; i < n_nodes; i++)
            {
                for (int j = 0; j < n_inputs; j++)
                {
                    if(Random.value < MutationChance)
                        weights[i, j] += Random.Range(-1.0f, 1.0f) * mutationAmount;
                }
                if(Random.value < MutationChance) 
                    biases[i] += Random.Range(-1.0f, 1.0f) * mutationAmount;
            }
        }
    }
    public void Awake() //viene avviata in automatico quando l'object con questo script viene creato
    {
        layers = new Layer[network.Length - 1]; // non conto il layer di input
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(network[i], network[i + 1]);
        }
        Mutate(0.5f, 0.5f);
    }
    public void Mutate(float mutationAmount, float MutationChance){
        foreach (Layer l in layers) l.Mutate(mutationAmount, MutationChance);
    }
    public float[] Brain(float[] inputs)
    {
        layers[0].Forward(inputs);
        layers[0].Activation();

        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].Forward(layers[i - 1].nodes);
            if(i < layers.Length - 1) layers[i].Activation(); //non faccio l'attivazione sul layer di output in modo da poter ottenere risultati negativi
        }

        return layers[layers.Length - 1].nodes;
    }
    public Layer[] CopyLayers()
    {
        Layer[] newLayers = new Layer[layers.Length];
        // copio i layer
        for (int i = 0; i < layers.Length; i++)
        {
            newLayers[i] = new Layer(network[i], network[i + 1]);
            System.Array.Copy(layers[i].weights, newLayers[i].weights, layers[i].weights.GetLength(0)*layers[i].weights.GetLength(1));
            System.Array.Copy(layers[i].biases, newLayers[i].biases, layers[i].biases.Length);
        }
        return newLayers;
    }
}