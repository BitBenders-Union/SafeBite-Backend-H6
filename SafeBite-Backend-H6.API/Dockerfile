FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root

# --- Install Tesseract OCR & dependencies and create expected library links ---
RUN apt-get update && apt-get install -y --no-install-recommends \
      libc6-dev \
      tesseract-ocr \
      libleptonica-dev \
      libtesseract-dev \
      tesseract-ocr-eng \
      tesseract-ocr-dan \
      tesseract-ocr-swe \
      tesseract-ocr-nor \
      tesseract-ocr-fra \
      && ldconfig \
      && mkdir -p /app/x64 \
      && cp /usr/lib/x86_64-linux-gnu/liblept.so.5      /app/x64/libleptonica-1.82.0.so \
      && cp /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so \
      && ln -sf /usr/lib/x86_64-linux-gnu/libdl.so.2    /usr/lib/x86_64-linux-gnu/libdl.so \
      && apt-get clean && rm -rf /var/lib/apt/lists/*

COPY ./tessdata ./tessdata
ENV TESSDATA_PREFIX=/app/tessdata

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["SafeBite-Backend-H6.API.csproj", "./"]
RUN dotnet restore "SafeBite-Backend-H6.API.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "SafeBite-Backend-H6.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "SafeBite-Backend-H6.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SafeBite-Backend-H6.API.dll"]