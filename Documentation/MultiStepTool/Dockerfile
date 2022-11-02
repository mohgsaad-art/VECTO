FROM ubuntu:18.04
RUN apt-get update
RUN apt-get -y  install texlive
RUN DEBIAN_FRONTEND=noninteractive apt-get -y install texlive-latex-extra 
RUN apt-get -y install wget
RUN wget https://github.com/jgm/pandoc/releases/download/1.19.1/pandoc-1.19.1-1-amd64.deb
RUN dpkg -i ./pandoc-1.19.1-1-amd64.deb