@echo off

echo Starting Help-file generation...


setlocal enabledelayedexpansion enableextensions
set LIST=
for /f %%f in (files.txt) do set LIST=!LIST! "%%f"


REM -- pandoc 1.19 -- pandoc --verbose  -f markdown+raw_html %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --mathjax=includes/mathjax.js -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js

REM -- pandoc 1.19 -- %LOCALAPPDATA%\Pandoc\pandoc --verbose  -f markdown+raw_html %LIST% -s  --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js

REM -- pandoc 1.19 -- %LOCALAPPDATA%\Pandoc\pandoc --verbose  -f markdown+raw_html %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js

REM  -- pandoc 2.17 --
%LOCALAPPDATA%\Pandoc\pandoc.exe --verbose -f markdown+raw_html -s --toc --toc-depth=2 --self-contained --section-divs --mathjax -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js -o help.html  -V "pagetitle:VECTO xEV User Manual" %LIST%


REM pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex  -c includes/style.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js
REM pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex="https://latex.codecogs.com/svg.latex?\large "  -c includes/style.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js

echo Generated outputfile: help.html

pause

REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.docx
REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.pdf
REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.latex
