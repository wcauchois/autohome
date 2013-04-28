#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <string.h>
#include <unistd.h>

int main(int argc, char **argv) {
  int server_sock;
  struct sockaddr_in my_addr;
  const char *device_name;
  int listen_port;

  if (argc < 3) {
    printf("Usage: %s PORT DEVICE\n", argv[0]);
    exit(0);
  }
  device_name = strdup(argv[2]);
  listen_port = atoi(argv[1]);

  server_sock = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
  if (-1 == server_sock) {
    perror("Error creating server socket");
    exit(1);
  }

  memset(&my_addr, 0, sizeof(my_addr));
  my_addr.sin_family = AF_INET;
  my_addr.sin_port = htons(listen_port);
  my_addr.sin_addr.s_addr = htonl(INADDR_ANY);

  if (-1 == bind(server_sock, (struct sockaddr *)&my_addr, sizeof(my_addr))) {
    perror("bind() failed");
    close(server_sock);
    exit(1);
  }

  if (-1 == listen(server_sock, 10)) {
    perror("listen() failed");
    close(server_sock);
    exit(1);
  }
  printf("Listening on port %d\n", listen_port);

  for (;;) {
    int client_sock;
    struct sockaddr client_addr;
    socklen_t addrlen = sizeof(client_addr);
    char addrstr[INET_ADDRSTRLEN];
    int childpid;
    int pipefd[2];

    client_sock = accept(server_sock, &client_addr, &addrlen);
    if (0 > client_sock) {
      perror("accept() failed");
      close(server_sock);
      exit(1);
    }
    inet_ntop(AF_INET, &((struct sockaddr_in *)&client_addr)->sin_addr, addrstr, INET_ADDRSTRLEN);
    printf("Accepted connection from %s\n", addrstr);

    if ((childpid = fork()) == 0) {
      dup2(client_sock, 0);
      execlp("pacat", "pacat", "-d", device_name, "--latency-msec", "10", (char *)0);
    } else {
      waitpid(childpid, NULL, NULL);
    }
    printf("Client disconnected\n");
  }

  close(server_sock);
  return 0;
}
