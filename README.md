# Meeting Room Booking

## 1. Run with Docker Compose

Install Docker Desktop, then run from the project root:

```powershell
docker compose up --build
```

Open the application at http://localhost:5173.

Stop the containers with:

```powershell
docker compose down
```

## 2. Run with npm

Install the .NET 10 SDK and Node.js, then run:

```powershell
cd frontend
npm install
npm run dev
```

Open the application at http://localhost:5173.

## 3. Run frontend and backend separately

Start the backend:

```powershell
dotnet run --project backend\MeetingRoomBooking.Api\MeetingRoomBooking.Api.csproj
```

In another terminal, start the frontend:

```powershell
cd frontend
npm install
npm run dev:frontend
```

The backend runs at http://localhost:5189.
