# LastLevelOpt con separazione basta sulla Label senza tenere in memoria il flusso del percorso

Il funzionamento dell'algoritmo è come quello di LastLevelOpt, ma non tengo in memoria il flusso passante con un certo percorso, dovendo calcolare ogni volta il flusso nel percorso scelto, ma non avendo problemi con flussi non propagati.
Al posto di InFlow, tengo conto se il nodo è già stato esplorato o meno.
