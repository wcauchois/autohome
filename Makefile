
bin/parelay: bin parelay/main.c
	gcc -o bin/parelay parelay/main.c

bin:
	mkdir -p bin
