# Algoritmo di Edmond-Karp

ricerco, tramite una BFS, un flusso da inviare, facendo sì che ogni nodo salvi quello da cui è arrivato (attributo previousNode), 
salvando anche la quantità di flusso che è possibile che passi da quel determinato percorso (InFlow nel codice, flussoPassante nello pseudocodice)
in questo caso si è utilizzato due archi per indicare l'andata e il ritorno, rispettivamente MonoEdge ed ReversMonoEdge
una volta che tramite la Bfs e trovato il nodo destinazione t, invio il flusso indicatomi, e ricomincio finché o non è più possibile trovare un percorso
 che collega il nodo Source s a t