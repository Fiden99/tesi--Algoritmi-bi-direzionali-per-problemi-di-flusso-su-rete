# LastLevelOpt con numero di nodi esplorati dalla sorgente pari a quelli esplorati dalla destinazione, salvando il flusso raggiunto fino a quel nodo di un dato percorso

## Strutture dati

### BiEdge

L'arco BiEdge che collega due nodi, che ha le informazioni sulla capacità residua e sulla quantità di flusso inviata, oltre a un booleano per capire se durante l'invio del flusso deve inviarlo o ritirarlo.

### Node

Il nodo contiene le informazioni sugli archi a lui collegati con una lista di BiEdge.
Inoltre salva la propria distanza dal nodo sorgente *s* e/o *t* (a seconda di sourceSide) , le informazioni per l'indirizzamento (previousEdge e previousNode per la parte esplorata da *s*, nextEdge e nextNode per la parte esplorata da *t*) e il flusso inviabile fino a quel nodo nel percorso indicato.
Infine, ci sono due booleani, uno che mi indica se il nodo è valido (cioè l'arco "precedente" ha capacità pari a 0), l'altro se è stato esplorato da *s* o da *t*.

### Graph

Graph è rappresentato da una lista di insiemi di nodi, cioè i nodi esplorati da *s* e da *t*, divisi per label, oltre ad avere due insiemi che contengono i nodi di confine fino a quel momento scoperti, sia dalla parte di *s*, sia dalla parte di *t*.

## Descrizione

svolgo una prima Bfs per inizializzare i nodi, in maniera tale che ho lo stesso numero di nodi sia da una parte sia dall'altra.
Successivamente, procede come lo stesso algoritmo descritto in NodePropagation (sempre lastLevelOpt) con le informazioni date dalla prima Bfs.

### FirstBfs

1. creo una coda di nodi di source, e gli accodo il nodo sorgente *s*
2. creo una seconda coda di nodi di sink, e gli accodo il nodo destinazione t
3. creo due code vuote di archi
4. se entrambe le code sono vuote, vado al punto 17
5. se c'è almeno un elemento in coda di source, con la coda di edge di source vuota (o che sia la coda dei nodi di sink che la coda di archi di edge siano vuote)
6. faccio la dequeue dalla coda dei nodi
7. inserisco nella coda di archi tutti gli archi uscenti dal nodo
8. ripeto i passi 5,6,7 per quanto riguarda la parte di sink.
9. se entrambe le code di archi non sono vuote, proseguo, altrimenti torno al passo 4
10. estraggo un arco dalla coda di archi di source
11. esploro il nodo indicatomi dall'arco, conoscendo il nodo di partenza (dal punto 6)
12. se incontro un valore esplorato da entrambi i nodi e non è già stato trovato un valore di flusso, salvo il nodo esplorato da sink e il valore del flusso
13. se necessario, aggiorno i valori del nodo, e lo inserisco nella coda
14. estraggo un arco dalla coda di archi di sink
15. ripeto i punti 11,12,13 per l'arco ottenuto
16. ritorno al punto 9
17. ritorno il valore del flusso salvato (oltre al nodo intermedio)
