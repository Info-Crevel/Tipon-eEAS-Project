
<RoutePrefix("api")>
Public Class IXS_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <Route("web-applicant/list")>
   <HttpPost>
   Public Function AddStsSurvey(<FromBody> applicants As HrsApplicantList) As IHttpActionResult

      Try

         Dim _applicantId As Integer = SysLib.GetNextSequence("ApplicantId")

         With DataCore.Command
            .Connection = DataCore.Connection
            .CommandText = "web.InsHrsApplicant"
            .CommandType = CommandType.StoredProcedure

            DataCore.Connect(_databaseId)

            For Each _applicant As HrsApplicant In applicants
               With .Parameters
                  .Clear()

                  .AddWithValue("@ApplicantId", _applicantId)
                  .AddWithValue("@MemberId", _applicant.MemberId.ToNullable)
                  .AddWithValue("@ApplicantLastName", _applicant.ApplicantLastName)
                  .AddWithValue("@ApplicantFirstName", _applicant.ApplicantFirstName)
                  .AddWithValue("@ApplicantMiddleName", _applicant.ApplicantMiddleName)
                  .AddWithValue("@ApplicantSuffixId", _applicant.ApplicantSuffixId)
                  .AddWithValue("@BirthDate", _applicant.BirthDate)
                  .AddWithValue("@SexId", _applicant.SexId)
                  .AddWithValue("@CivilStatusId", _applicant.CivilStatusId)
                  .AddWithValue("@ReligionId", _applicant.ReligionId)
                  .AddWithValue("@Address1", _applicant.Address1)
                  '.AddWithValue("@Address2", _applicant.Address2)
                  .AddWithValue("@PostalCode", _applicant.PostalCode)
                  .AddWithValue("@PhoneNumber", _applicant.PhoneNumber)
                  .AddWithValue("@MobileNumber", _applicant.MobileNumber)
                  .AddWithValue("@Email", _applicant.Email)
                  .AddWithValue("@ApplicationSourceId", _applicant.ApplicationSourceId)
                  .AddWithValue("@ApplicationDate", _applicant.ApplicationDate)
                  .AddWithValue("@ApplicantStatusId", ApplicantStatus.Active)
                  .AddWithValue("@RegionId", _applicant.RegionId)
                  .AddWithValue("@ProvinceId", _applicant.ProvinceId)
                  .AddWithValue("@MunicipalityId", _applicant.MunicipalityId)
                  .AddWithValue("@BarangayId", _applicant.BarangayId)
                  .AddWithValue("@MemberRequestId", _applicant.MemberRequestId)

               End With

               .ExecuteNonQuery()

            Next

            DataCore.Disconnect()

         End With

         Return Me.Ok(New With {
             .status = 200,
             .message = "Operation Completed."
         })

      Catch _exception As Exception
         If _exception.InnerException IsNot Nothing Then
            Return Me.BadRequest(_exception.Message + " " + _exception.InnerException.Message)
         Else
            Return Me.BadRequest(_exception.Message)
         End If
      End Try

   End Function

End Class
Public Class HrsApplicantBody
   Inherits HrsApplicant

End Class

Public Class HrsApplicantList
   Inherits List(Of HrsApplicant)

End Class
