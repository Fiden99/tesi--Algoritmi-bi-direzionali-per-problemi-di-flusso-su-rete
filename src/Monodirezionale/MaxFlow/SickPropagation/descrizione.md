# SickPropagation, propagazione della malattia

## strutture dati

### BiEdge

I nodi vengono collegati tra di loro da archi BiEdge, che contiene le informazioni da quale nodo esce e in quale nodo entra.
Ovviamente conserva in memoria la quantità di flusso che passa e la sua capacità residua, oltre a un booleano (reversed) che mi indica che quel arco, durante l'invio del flusso, dovrà inviarlo o ritirarlo.

### Node

Node contiene al suo interno informazioni sugli archi a lui collegati (Lista di BiEdge), un booleano che indica se il nodo è valido o meno,le indicazioni sul percorso che deve fare, quindi sia previousNode e previousEdge, la label, cioè la distanza tra lui e il nodo sorgente *s*, oltre a un booleano che mi indica se è stato visitato o meno.

### Graph

Contiene un insieme per i nodi non validi e una lista di insieme che contiene i nodi divisi per label

## descrizione

il funzionamento è molto simile a quello di LastLevelOpt, con una differenza in fase di "inizializzazione".
Dopo aver controllato che non è possibile riparare il nodo non valido **noCap**  e trovare un percorso senza nodi non validi, procedo con SickPropagation, nel caso riesco a trovare un percorso, restituisco il valore, altrimenti cancello tutte le informazioni dai nodi con label pari o maggiori a quella **noCap** e, nel caso in cui SickPropagation non abbia già inserito dei nodi in coda, inserisco i nodi con label = **noCap**.label-1, poi procedo come se fosse LastLevelOpt

### SickPropagation

provo a riparare il nodo che mi è stato indicato, per primo proprio **noCap**, nel caso non si riesca a ripararlo, inserisco in una coda tutti i nodi che lo hanno come predecessore, se invece riesco a ripararlo lo inserisco nella coda che userò per esplorare il grafo, a meno che non sia *t*, in tal caso ritorno il valore del flusso che riesco a inviare.
