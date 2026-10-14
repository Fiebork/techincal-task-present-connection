FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MeetingRoomBooking.Api/MeetingRoomBooking.Api.csproj", "MeetingRoomBooking.Api/"]
RUN dotnet restore "MeetingRoomBooking.Api/MeetingRoomBooking.Api.csproj"

COPY . .
WORKDIR "/src/MeetingRoomBooking.Api"
RUN dotnet restore "MeetingRoomBooking.Api.csproj"
RUN dotnet publish "MeetingRoomBooking.Api.csproj" \
    --configuration Release \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5189
EXPOSE 5189

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MeetingRoomBooking.Api.dll"]
