# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

R5 nu era prezent deloc in starter. Acum este prezent in Domain/Bookings/Booking.cs, prin intermediul metodei Create, care nu permite creerea unui Booking now fara ca niste conditii sa se respecte, una dintre acestea fiind aceasta regula impusa ca un Booking nu poate incepe in trecut

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Result permite tratarea explicita a erorile previzibile. Handler-ul detecteaza eroarea si transmite codul http si eroarea corespunzatoare

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Stiu de EF core toate clasele din Infrastructure/Persistence/, nu stiu de EF Core Domain/ si Features/. Domeniul poate fi testat fara baza de date, persistenta poate fi schimbata

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

CreateBookingEndpoint (cu CreateBookingRequest) → CreateBookingCommand → CreateBookingHandler
IRoomRepository.GetByIdAsync (RoomRepository) → daca nu exista, 404
TimeProvider.GetUtcNow()
IBookingRepository.GetActiveForRoomAsync si GetActiveForParticipantOnDayAsync (BookingRepository)
Booking.Create, care foloseste si TimeRange
IBookingRepository.Add
IUnitOfWork.SaveChangesAsync, adica RoomHubDbContext, care scrie in roomhub.db (SQLite)
Apoi CreateBookingResponse → 201 + Location. Pe ramura de eroare: ErrorHttpMapping.ToProblem, care da ProblemDetails.

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

Initial, AI-ul avea o interfata in interiorul altei clase, i-am cerut sa refactorizeze codul astfel incat sa fie totul separat, sa nu mai apara constructii de genul. De asemenea rutele nu respectau versionarea initial 

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

As face verificari mult mai ample, timpul e unul foarte limitat, chiar si cu unelte AI, tot trebuie sa fii foarte atent la ce este generat si task-ul de scriere manuala a unor fisiere a mancat timp pe care l-as fi putut folosi in verificarea mai ampla a codului generat.
