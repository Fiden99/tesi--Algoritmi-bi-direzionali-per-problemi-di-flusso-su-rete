# No Opt bidirezionale con separazione basata sulla label

## Strutture dati

### BiEdge

L'arco BiEdge che collega due nodi, che ha le informazioni sulla capacità residua e sulla quantità di flusso inviata, oltre a un booleano per capire se durante l'invio del flusso deve inviarlo o ritirarlo.

### Node

Il nodo contiene le informazioni sugli archi a lui collegati con una lista di biEdge.
Inoltre salva la propria distanza dal nodo sorgente *s* (il nodo destinazione *t* ha il valore massimo consentito (int.MaxValue), quindi c'è un gap tra i due dati, andrebbe, in fase finale, andrebbero corrette le label), le informazioni per l'indirizzamento (previousEdge e previousNode per la parte esplorata da *s*, nextEdge e nextNode per la parte esplorata da *t*).
Infine, ci sono due booleani, uni che mi indica se il nodo è stato precedentemente esplorato o meno, l'altro se è stato esplorato da *s* o da *t*.

### Graph

Il grafo è rappresentato da due insieme di nodi, in uno sono presenti quelli esplorati da source, nel secondo quelli esplorati da sink.

## Descrizione  

effettuo una bfs che sia basa sulla label (descritta in seguito), dicendogli se deve lavorare sulla parte di source o su quella di sink (o su entrambe), per poi inviare il flusso capire la quantità di flusso inviabile e, successivamente, inviarlo, salvando dove un arco non può più inviare flusso.
Se non trovo il percorso o il flusso inviabile è pari a 0, termino l'algoritmo.

### Bfs

Riceve in input due booleani, **SourceSide** e **SinkSide** per indicare quale parte del grafo devo esplorare.
Cancello i dati di indirizzamento e il fatto di averlo visitato, caricando nella coda apposita il nodo *s* e/o *t*.
Esploro tutti i nodi presenti nella coda apposita, caricando i nodi esplorati in una coda buffer (onde evitare di esplorarli in quella esecuzione).
Dopo aver "esplorato" tutti gli elementi presenti nella coda, faccio uno swap (a livello di puntatore) tra buffer (con i nodi interessati) e la coda usata (vuota), per poi procedere come la coda successiva.
Per l'esplorazione, consideriamo di ottenere dalla coda un nodo **n**, se gli archi sono uscenti da **n** e hanno capacità positiva, controllo se sono stati esplorati, e nel caso lo fossero controllo se sono stai esplorati dalla "stessa parte", quindi non considerandolo, o se viene esplorato dalla "parte opposta", quindi indicandomi che ho trovato un percorso, nel caso non fossero stati esplorati, aggiorno i dati con le informazioni di **n** e li inserisco nella coda buffer.
stessa cosa per gli archi entranti in **n** con flusso positivo.