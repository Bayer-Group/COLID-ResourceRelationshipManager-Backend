#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN apt-get install ca-certificates -y
RUN update-ca-certificates


WORKDIR /src

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore src/COLID.ResourceRelationshipManager/COLID.ResourceRelationshipManager.WebApi.csproj 

# build app
WORKDIR /src/src/COLID.ResourceRelationshipManager
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

ARG BUILD_CIPIPELINEID
ENV Build__CiPipelineId=${BUILD_CIPIPELINEID}

ARG BUILD_CICOMMITSHA
ENV Build__CiCommitSha=${BUILD_CICOMMITSHA}

ENV PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE $PORT
WORKDIR /app
COPY --from=build /app ./
CMD ["dotnet", "COLID.ResourceRelationshipManager.WebApi.dll"]