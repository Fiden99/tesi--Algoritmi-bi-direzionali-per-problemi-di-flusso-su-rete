# LastLevelOpt con separazione basta sulla Label

## Strutture dati

### BiEdge

L'arco BiEdge che collega due nodi, che ha le informazioni sulla capacità residua e sulla quantità di flusso inviata, oltre a un booleano per capire se durante l'invio del flusso deve inviarlo o ritirarlo.

### Node

Il nodo contiene le informazioni sugli archi a lui collegati con una lista di BiEdge.
Contiene un intero InFlow, che rappresenta il valore di flusso che è possibile inviare nel percorso predefinito, oltre a un intero Label, che rappresenta la distanza tra quel nodo e *s* o *t*, a seconda da quale parte è stato esplorato.
Due booleani, il primo mi rappresentano se è stato esplorato, il secondo se l'arco che lo collega con il nodo precedente (e quindi più vicino a *s*/*t*) ha capacità =  0.
Inoltre ci sono le informazioni di indirizzamento (nextNode e nextEdge per i nodi esplorati da *t*, previousNode e previousEdge per i nodi esplorati da *s*).

### Graph

Graph è rappresentato da una lista di insiemi di nodi, cioè i nodi esplorati da *s* e da *t*, divisi per label, oltre ad avere due insiemi che contengono i nodi di confine fino a quel momento scoperti, sia dalla parte di *s*, sia dalla parte di *t*.

## Descrizione

La fase di invio del flusso e il tentativo di recuperare il percorso riparando il nodo con noCapSource o noCapSink è come in NodePropagation. <!-- copia e incolla di quello già scritto-->

Finché riesco a trovare un percorso *s* e *t*, cerco il percorso tramite la Bfs ricercata,e poi inviare il flusso.
Nell'invio del flusso, seleziono, se presente, il nodo che ha come capacità residua = 0,e uso quel nodo per la Bfs (questo per entrambi i lati esplorati).

Dato che analizzo l'InFlow presente, è possibile che non sia adeguatamente aggiornato, quindi nel caso in cui non è possibile inviare il flusso indicato, faccio tornare i nodi a come erano prima dell'invio e la partenza è indicata dal nodo dove ho rincontrato il problema.

### Bfs con ottimizzazione sugli ultimi livelli

gli passo in input, oltre al grafo, anche due nodi **noCapSource** e **noCapSink**.

#### riparazione di noCapSource

nel caso in cui **noCapSource** non sia null, provo a ripararlo, nel caso in cui riesca, controllo se **noCapSink**, se è null controllo che il percorso descritto  tra uno dei lastSinkNodes (questo è dovuto che sono i nodi di confine con sia i valori previousNode sia di nextNode) e **noCapSource** abbia tutti i nodi validi (quindi è possibile inviare del flusso), e se lo trovo ho già concluso la ricerca di un nuovo percorso.
nel caso invece **noCapSink** sia un valore, salvo il fatto che **noCapSource** è stato riparato.
accodo i valori opportuni, quindi o *s* se è la prima volta,i valori di LastSourceNodes se è un nodo di confine (quindi appartenente quelli "esplorati" da sink), oppure  i valori con label precedente, in questo caso eliminando le informazioni di indirizzamento e inflow dei nodi con label pari o superiore a **noCapSource**

#### riparazione di noCapSink

nel caso in cui **noCapSink** non sia null, provo a ripararlo.
Se riesco, salvo il fatto che sono riuscito a ripararlo, e controllo se **noCapSource** è stato riparato o è null.
per ogni nodo presente in LastSinkNodes procede come segue

- nel caso in cui **noCapSource** sia stato riparato, controllo che il nodo selezionato raggiunga **noCapSource** senza incontrare nodi non validi (quindi con flusso inviabile positivo), salvando la quantità di flusso inviabile, altrimenti proseguo col nodo successivo.
- nel caso in cui **noCapSource** sia null, salvo il minimo tra la quantità di flusso che è raggiunge il nodo selezionato (l'InFlow) e la capacità/flusso dell'arco che lo collega con **noCapSink**

Terminato questi due casi, controllo se il flusso inviabile è positivo prima indicato ed se è possibile, dal nodo selezionato,raggiungere **noCapSink** senza incontrare nodi non validi (quindi inviare del flusso).
Nel caso sia possibile, ho trovato un percorso dove è possibile inviare il flusso.

Se non è stato trovato il percorso, accodo o *t* o i valori con label precedente a quella di **noCapSink**, resettando le informazioni di indirizzamento e InFlow dei nodi con label pari o superiore a quella di **NoCapSink**.

#### ricerca del percorso

per ogni coda,esamino tutti gli elementi al suo interno ed esploro i nodi a loro collegati, nel caso siano stati esplorati dalla "parte opposta"  e siano validi vuol dire ho trovato il percorso,altrimenti se sono stati esplorati e esplorati dallo stesso nodo, esploriamo un nuovo nodo (di quelli collegati al nodo estratto dalla coda).
Nel caso non risulti ancora esplorato, aggiorno i sui dati dal nodo estratto dalla coda e lo aggiungo a una coda buffer.
Prima di procedere ad esplorare la  nuova coda, eseguo uno swap (di puntatori) tra buffer, con i nodi che sono stati aggiornati, e la coda che stavo usando, vuota.
Proseguo fino a trovare un percorso.

### RepairNode

prendo in input un nodo e un booleano per capire da quale parte deve essere considerato nel caso in cui tratto un nodo "di confine"

nel caso un cui node sia stato esplorato da source, cerco tra i nodi a lui collegati un nodo tale che sia valido, con capacità/flusso positivo e label uguale a quella del nodo -1 (nodeInput.label = nodoEsplorato.label + 1)
nel caso lo trovo, salvo, aggiorno i dati e confermo che ho riparato il nodo.

nel caso in cui il nodo in input sia stato esplorato da sink, devo controllare anche il booleano nel caso il valore sia di confine.

- se il nodo era di confine e il booleano mi dice che deve esplorare la parte di source, cerco se tra i nodi appartenenti a lastSourceNodes è possibile ripararlo
- altrimenti cerco un nodo valido con flusso/capacità positiva e con label pari a quella del nodo in ingresso -1 (nodeInput.label = nodoEsplorato.label+1)
  
nel caso riesco a trovarlo, aggiorno i dati e confermo di averlo trovato.
