# Kapsayıcıları oluşturan veya çalıştıran konak makinelerinin işletim sistemine bağlı olarak FROM deyiminde belirtilen görüntünün değiştirilmesi gerekir.
# Daha fazla bilgi edinmek için https://aka.ms/containercompat sayfasına bakın

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
ARG source
WORKDIR /inetpub/wwwroot
COPY ${source:-obj/Docker/publish} .
