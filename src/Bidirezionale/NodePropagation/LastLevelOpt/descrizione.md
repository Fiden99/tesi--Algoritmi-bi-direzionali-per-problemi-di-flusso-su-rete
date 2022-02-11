# LastLevelOpt bidirezionale con propagazione dei nodi

## Strutture dati

### BiEdge

arco BiEdge che collega due nodi, che ha le informazioni sulla capacità residua e sulla quantità di flusso inviata, oltre a un booleano per capire se durante l'invio del flusso deve inviarlo o ritirarlo.

### Node

Il nodo node contiene tutti gli archi a lui collegato come una lista di BiEdge, inoltre contiene informazioni per quanto riguarda il proprio indirizzamento per l'invio del flusso, tramite gli attributi previousNode e previousEdge per quanto riguarda la parte esplorata dal nodo sorgente *s*, e nextNode e nextEdge per quanto riguarda la parte esplorata dal nodo destinazione *t*, e un booleano sourceSide, per capire da chi è stato esplorato.
Inoltre tiene conto della label, quindi di quanto è distante o da *s* o da *t* (a seconda da chi è stato esplorato), e il booleano valid, che mi indica se l'arco precedente del percorso ha valore pari a zero o meno.
Infine contiene un attributo per tenere traccia del flusso passante per quel nodo, cioè InFlow (usato anche per vedere se è stato esplorato o meno).

### Graph

Graph contiene due liste di insiemi, contenenti i nodi esplorati da *t* e *s*, divisi per la diversa label contenuta, inoltre contiene un insieme dei nodi "a confine", quindi i nodi che esplorati da sink che si collegano con i nodi di source, e viceversa.

## Descrizione

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

Se non è stato trovato il percorso, accodo o *t* o i valori con label precedente a quella di **noCapSink**, resettando le informazioni di indirizzamento e InFlow dei nodi con label pari o superiore a quella di **NoCapSink**

#### ricerca del percorso

La ricerca avviene come in NoOpt, quindi:

Finché entrambe le code non sono vuote, procedo ad analizzare tutti i nodi, ancora da esplorare, collegati al nodo che ho ottenuto dalla dequeue della coda interessata (per poi accodarli alla coda usata per ottenere quel valore), per poi passare all'altra coda, se possibile.
Quando un nodo incontra un nodo esplorato "dall'altra parte", restituisco il valore del nodo (aggiornato) appartenente ai nodi esplorati da Sink (scelta arbitraria fatta).

### RepairNode

prendo in input un nodo e un booleano per capire da quale parte deve essere considerato nel caso in cui tratto un nodo "di confine"

nel caso un cui node sia stato esplorato da source, cerco tra i nodi a lui collegati un nodo tale che sia valido, con capacità/flusso positivo e label uguale a quella del nodo -1 (nodeInput.label = nodoEsplorato.label + 1)
nel caso lo trovo, salvo, aggiorno i dati e confermo che ho riparato il nodo.

nel caso in cui il nodo in input sia stato esplorato da sink, devo controllare anche il booleano nel caso il valore sia di confine.

- se il nodo era di confine e il booleano mi dice che deve esplorare la parte di source, cerco se tra i nodi appartenenti a lastSourceNodes è possibile ripararlo
- altrimenti cerco un nodo valido con flusso/capacità positiva e con label pari a quella del nodo in ingresso -1 (nodeInput.label = nodoEsplorato.label+1)
  
nel caso riesco a trovarlo, aggiorno i dati e confermo di averlo trovato.
