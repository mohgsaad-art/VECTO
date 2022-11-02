docker build -t ubuntu_pandoc .
docker run --rm --volume /mnt/c/Users/Harry/source/repos/hm_vecto-dev/Documentation/MultiStepTool:/documentation --entrypoint /documentation/convert.sh ubuntu_pandoc
