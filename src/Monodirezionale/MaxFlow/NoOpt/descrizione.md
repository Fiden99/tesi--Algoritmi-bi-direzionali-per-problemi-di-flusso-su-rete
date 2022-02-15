# Algoritmo di Edmond-Karp

## Strutture dati

### MonoEdge

Contiene l'indicazione del nodo successivo, la capacità residua e il flusso inviato.
È inoltre presente una sottoclasse **ReversedMonoEdge** per l'arco contrario.
Per l'invio del flusso devo trovare entrambi gli archi tra quelli collegati al nodo di invio.

### Node

Node ha le seguenti proprietà

- la lista di archi a lui connessi (in fase di inserimento inserisca sia MonoEdge sia ReversedMonoEdge)
- label, per indicare la distanza da s
- PreviousNode, che mi indica il nodo precedente per tornare da s, in maniera da indicarmi il percorso
- il flussoPassante (InFlow nel codice), che mi indica, attraverso il percorso dato, fino a quel nodo quanto flusso è inviabile (proseguendo potrebbe diminuire)
<!-- per motivi di compressione e debug, è presente anche un nome -->

### Graph

Insieme di Node presenti nel grafo.

## Descrizione

Ricerco, tramite una BFS, un flusso da inviare, facendo sì che ogni nodo salvi quello da cui è arrivato e la quantità di flusso inviabile da quello specifico percorso.
Una volta che tramite la Bfs e trovato il nodo destinazione t, invio il flusso indicatomi, e ricomincio finché o non è più possibile trovare un percorso che collega il s e t.
