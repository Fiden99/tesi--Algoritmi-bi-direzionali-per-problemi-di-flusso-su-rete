# LastLevelOpt

cioè Algoritmo di Edmond-Karp con ottimizzazione per quanto riguarda gli ultimi livelli

## Descrizione

dopo aver trovato un percorso tra il nodo source e il nodo Sink, mentre invio il flusso, controllo che la capacità residua degli archi sia maggiore
di zero, in caso la capacità sia pari a 0, il nodo successivo viene marcato come non valido, inoltre viene salvato l'ultimo nodo non valido visitato,
che corrisponde al quello più vicino a s (dato che l'invio del flusso va da t verso s), chiamato qui **malato**
da qui abbiamo due possibilità

### RepairNode

è possibile "guarire il nodo" **malato**, cioè cercare un nodo **sostituto** a lui collegato, che deve avere le seguenti proprietà

1. l'arco tra **malato** e **sostituto** deve avere capacità (o flusso) positivo, cioè maggiore di zero
2. la label di **sostituto** deve essere uguale alla label di **malato** -1, cioè
**sostituto**.Label = **malato**.Label-1
3. **sostituto** deve essere un nodo valido

nel caso sia riuscito a riparare il nodo, calcolo quanto flusso posso inviare nel percorso trovato, nel caso sia positivo, cioè non ci sono nodi non validi, mando il flusso indicato e ripeto l'algoritmo.

### ottimizzazione degli ultimi livelli

se non sono riuscito a riparare **malato**, posso procedo a inserire tutti i nodi tale per cui la loro label è pari a **malato**.label-1 (in Graph salvo i nodi ivi contenuti in delle insiemi (da capire se si dovranno cambiare da HashSet in List per motivi prestazionali)) nella coda che verrà utilizzata per fare la bfs.
Controllo che i nodi che vado a esplorare non siano già stati esplorati (o che non siano validi), in tal modo mi assicuro che proceda solo da s verso t.

### strutture dati utilizzate

sono stati usate i nodi, degli oggetti, collegati da arco BiEdge, tale che contiene le informazioni sia da chi esce sia verso chi entra, senza dover aver bisogno di modificare, durante l'invio del flusso, due archi ma solo uno.
Graph è rappresentato da un Lista di vari insiemi, ogni insieme contiene i nodi che hanno una certa label, oltre ad avere un insieme di nodi non validi,
