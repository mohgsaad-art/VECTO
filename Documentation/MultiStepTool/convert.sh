#!/bin/bash


#readarray [-d delim] [-n count] [-O origin] [-s count]
 #   [-t] [-u fd] [-C callback] [-c quantum] [array]
#ls

cd documentation

#ls

declare -a FILES
mapfile -t FILES < files.txt
cat files.txt
echo "Running pandoc with these input files"
echo "${FILES[@]}"

OUTPUTFILE="MultiStepToolHelp.html"

#pandoc --verbose  -f markdown+raw_html %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -o HashingToolHelp.html  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js
pandoc --verbose -f markdown+raw_html ${FILES[@]} -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -o $OUTPUTFILE  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js


