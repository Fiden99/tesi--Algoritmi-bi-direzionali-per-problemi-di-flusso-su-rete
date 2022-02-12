# LastLevelOpt senza salvataggio del flusso

l'algoritmo è come quello di LastLevelOpt, senza salvare il flusso che passa per quel nodo.

La differenza, quindi, sta nel fatto che ogni volta che devo inviare il flusso devo ricavarne il valore, essendo sicuro che sia quello, al contrario in LastLevelOpt è possibile che non è possibile mandare tutto il flusso richiesto,e quindi devo ricalcolare una parte, nonostante abbia già il percorso.
<!-- da valutare in un grafo di grandi dimensioni quale dei due conviene usare, anche in vista dello sviluppo di SickPropagation-->