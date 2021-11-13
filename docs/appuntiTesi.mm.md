# appunti tesi

## algoritmo di ricerca

### [man in the middle (MM)](https://ojs.aaai.org/index.php/AAAI/article/view/10436)

#### euristico, potrebbe essere problematico

### [Front-to-front GPU bidirectional search (FFGBS)](https://link.springer.com/article/10.1007/s10589-021-00303-5#Sec6)

#### CPU + GPU

#### capire se ha bisogno di euristica

## come poter fare per farlo bidirezionale

### capire se, trovando il taglio minimo, riesco a risalire anche al flusso massimo

#### [Parallel Implementations of Gusfield’s Cut Tree Algorithm](https://link.springer.com/content/pdf/10.1007%2F978-3-642-24650-0.pdf)

##### [gomory-hu tree](https://epubs.siam.org/doi/10.1137/0109047)

##### non tratta di bidirezionale, ma potrebbe essere un buon punto di partenza

##### [Gusfield's alghoritm](https://epubs.siam.org/doi/abs/10.1137/0219009)

##### [Hao-Oriln, taglio minimo con nodi s-t fissi](https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.95.2427&rep=rep1&type=pdf)

### [capire se algoritmo di Orlin (01/06/2013) potrebbe servire, riducendo la dimensione della rete](https://dl.acm.org/doi/pdf/10.1145/2488608.2488705)

### capire come ricalcolare le label

### algoritmi push-relabel

#### capire come funzionano

#### capire se potrebbero essere utili alla causa

## possibili librerie utilizzabili
### simil C/ per C

#### [paper su maxflow parallelo](https://dl.acm.org/doi/pdf/10.1145/1989493.1989511)

### per C++

#### [openMP](https://www.openmp.org/)

#### [MPI](https://mpitutorial.com/tutorials/mpi-introduction/)

### ?RUST?