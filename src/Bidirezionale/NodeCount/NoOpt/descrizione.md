# NoOpt con numero di nodi esplorati dalla sorgente pari a quelli esplorati dalla destinazione

## Strutture dati

### BiEdge

L'arco BiEdge che collega due nodi, che ha le informazioni sulla capacità residua e sulla quantità di flusso inviata, oltre a un booleano per capire se durante l'invio del flusso deve inviarlo o ritirarlo.

### Node

Il nodo contiene le informazioni sugli archi a lui collegati con una lista di BiEdge.
Inoltre salva la propria distanza dal nodo sorgente *s* (il nodo destinazione *t* ha il valore massimo consentito (int.MaxValue), quindi c'è un gap tra i due dati che, in fase finale, andrebbe corretto in maniera tale che le label abbiano valori consecutivi), le informazioni per l'indirizzamento (previousEdge e previousNode per la parte esplorata da *s*, nextEdge e nextNode per la parte esplorata da *t*).
Infine, ci sono due booleani, uni che mi indica se il nodo è stato precedentemente esplorato o meno, l'altro se è stato esplorato da *s* o da *t*.

### Graph

Il grafo è rappresentato da due insieme di nodi, in uno sono presenti quelli esplorati da source, nel secondo quelli esplorati da sink.

## Descrizione

Effettuo una bfs bidirezionale esplorando lo stesso numero di nodi da entrambe le parti, dicendogli se deve lavorare sulla parte di source o su quella di sink (o su entrambe), per poi inviare il flusso capire la quantità di flusso inviabile e, successivamente, inviarlo, salvando dove un arco non può più inviare flusso

Se non trovo il percorso o il flusso inviabile è pari a 0, termino l'algoritmo.

## Bfs

Dalle informazioni ricevute, cancello le informazioni riguardante la parte di source e/o sink, inserendo nell'apposita coda il nodo *s* e/o *t*.
Creo due code di archi BiEdge, una per source e una per sink.
Nel caso sia interessata solo una parte del grafo, inserisco nella coda degli archi dell'altra parte un arco, in maniera tale da rendere la coda non vuota

Finché entrambe le code non sono vuote procedo come segue

  1. se  c'è almeno un elemento nella coda di nodi esplorati da source, controllo che la coda degli archi della parte di source sia vuota o che entrambe le code di sink siano vuote proseguo al prossimo punto,altrimenti vado al punto 5.
  2. recupero il nodo **ns** dalla coda dei nodi
  3. per ogni arco entrante e uscente da **ns**, controllo che il nodo che non è **ns** sia esplorabile (quindi non precedentemente esplorato, a meno che non sia stato esplorato da sink, e con capacità/flusso positivo
  4. accodo dentro la coda degli archi di source i nodi indicatimi
  5. ripeto i primi 4 punti per i nodi di sink
  6. finché una coda degli archi non è vuota, recupero dalla coda un arco, ed esploro ed accodo nell'apposita coda il nodo, consapevole di quello di partenza, prima per quelli di source e poi per quelli di sink
  7. Finché entrambe le code di nodi non sono vuote, ritorno al punto 1.
