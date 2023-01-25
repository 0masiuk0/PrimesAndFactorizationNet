My library for functions related primes and factoriztions. Found primes are cached into hash table.
I made some small precautions for multi-thread use of primes chache but never tried multithreading yet.
I tryed to write everything in UInt64, but in a very limited fashion tested it beyond int32 range.

Optimized prime-finding sieves are curtesy of https://habr.com/ru/post/526924/;
