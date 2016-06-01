@echo off

:: Create private key called key.pem
2>NUL openssl genrsa -out priv.tmp 2048
2>NUL openssl pkcs8 -topk8 -in priv.tmp -nocrypt -out key.pem
del priv.tmp

:: Generate string to be used as "key" in manifest.json
2>NUL openssl rsa -in key.pem -pubout -outform DER -out pub.tmp
2>NUL openssl base64 -A -in pub.tmp -out extension.key
del pub.tmp

:: Calculate extension ID
2>NUL openssl rsa -in key.pem -pubout -outform DER -out pub.tmp
2>NUL openssl dgst -sha256 -out checksum.tmp pub.tmp
SET /p EXTID=<checksum.tmp
del checksum.tmp pub.tmp

SET EXTID=%EXTID:* =%
SET EXTID=%EXTID:~0,32%
SET EXTID=%EXTID:f=p%
SET EXTID=%EXTID:e=o%
SET EXTID=%EXTID:d=n%
SET EXTID=%EXTID:c=m%
SET EXTID=%EXTID:b=l%
SET EXTID=%EXTID:a=k%
SET EXTID=%EXTID:9=j%
SET EXTID=%EXTID:8=i%
SET EXTID=%EXTID:7=h%
SET EXTID=%EXTID:6=g%
SET EXTID=%EXTID:5=f%
SET EXTID=%EXTID:4=e%
SET EXTID=%EXTID:3=d%
SET EXTID=%EXTID:2=c%
SET EXTID=%EXTID:1=b%
SET EXTID=%EXTID:0=a%
echo %EXTID% > extension.id

