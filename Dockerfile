FROM mcr.microsoft.com/dotnet/sdk:2.1

RUN \
    echo "Install base packages" \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
    gnupg \
    make

WORKDIR /var/project
