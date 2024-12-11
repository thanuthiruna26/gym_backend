﻿using MaxFitGym.Entities;
using MaxFitGym.IRepository;
using MaxFitGym.Models;
using MaxFitGym.Models.RequestModel;
using MaxFitGym.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Net;
using System.Reflection;

namespace MaxFitGym.Repository
{
    public class MemberRepository: IMemberRepository
    {
        private readonly string _connectionString;

        public MemberRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public  ICollection<MemberResponseDTO> GetAllMembers()
        {
            var MemberList = new List<MemberResponseDTO>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id,Nic,FirstName,LastName,Password,DOB,ContactNumber,Email,Address,Age,Gender,Height,Weight,CreationDate,is_initalfeePaid,MembershipType,Fees  FROM MembersDetails";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MemberList.Add(new MemberResponseDTO()
                        {
                            Id = reader.GetInt64(0),
                            Nic = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            password = reader.GetString(4),
                            DOB = reader.GetDateTime(5),
                            ContactNumber = reader.GetString(6),
                            Email = reader.GetString(7),
                            Address = reader.GetString(8),
                            Age = reader.GetInt32(9),
                            Gender = reader.GetString(10),
                            Height = reader.GetInt32(11),
                            Weight = reader.GetInt32(12),
                            CreationDate = reader.GetDateTime(13),
                            is_initalfeePaid = reader.GetBoolean(14),
                            Membershiptype=reader.GetString(15),
                            fee = reader.GetInt32(16),
                        });
                    }
                }

            }
            return MemberList;

        }

        public MemberResponseDTO AddMember(MemberRegisterRequestDTO memberRegister)
        {
          
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
        INSERT INTO MembersDetails 
        (Nic, FirstName, LastName, Password, DOB, ContactNumber, Email,Address, Age, Gender, Height, Weight, CreationDate, is_initalfeePaid, MembershipType, Fees) 
        VALUES 
        (@nic, @firstname, @lastname, @password, @dob, @contactnumber, @email,@Address, @age, @gender, @height, @weight, @creationDate, @isinitialamountpaid, @MembershipType, @Fees) RETURNING Id;
       ";

                command.Parameters.AddWithValue("@nic", memberRegister.Nic);
                command.Parameters.AddWithValue("@firstname", memberRegister.FirstName);
                command.Parameters.AddWithValue("@lastname", memberRegister.LastName);
                command.Parameters.AddWithValue("@password", memberRegister.password); // Note the case change here
                command.Parameters.AddWithValue("@dob", memberRegister.DOB);
                command.Parameters.AddWithValue("@contactnumber", memberRegister.ContactNumber);
                command.Parameters.AddWithValue("@email", memberRegister.Email);
                command.Parameters.AddWithValue("@Address", memberRegister.Address);
                command.Parameters.AddWithValue("@age", memberRegister.Age);
                command.Parameters.AddWithValue("@gender", memberRegister.Gender);
                command.Parameters.AddWithValue("@height", memberRegister.Height);
                command.Parameters.AddWithValue("@weight", memberRegister.Weight);
                command.Parameters.AddWithValue("@creationDate", memberRegister.CreationDate);
                command.Parameters.AddWithValue("@isinitialamountpaid", memberRegister.is_initalfeePaid);
                command.Parameters.AddWithValue("@MembershipType", memberRegister.Membershiptype); // Ensure this matches the property name
                command.Parameters.AddWithValue("@Fees", memberRegister.fee); // Ensure this matches the property name

                var id = (long)command.ExecuteScalar();

                MemberResponseDTO memberResponseDTO = new MemberResponseDTO();


                memberResponseDTO.Id = id;
                memberResponseDTO.FirstName = memberRegister.FirstName;
                memberResponseDTO.LastName = memberRegister.LastName;
                memberResponseDTO.Nic = memberRegister.Nic;
                return memberResponseDTO;
            }



        }


        public void DeleteMember(int memberId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM MembersDetails WHERE Id == @id";
                command.Parameters.AddWithValue("@id", memberId);
                command.ExecuteNonQuery();
            }
        }


        public void UpdateMember(int memberId, MemberUpdateRequestDTO memberUpdate)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE MembersDetails SET FirstName = @firstname , LastName = @lastname , DOB = @dob , ContactNumber = @contactnumber , Email = @email ,Address=@Address, Age = @age , Gender = @gender , Height = @height , Weight = @weight,MembershipType=@membershiptype ,Fees=@newFees  WHERE Id == @id ";
                command.Parameters.AddWithValue("@firstname", memberUpdate.FirstName);
                command.Parameters.AddWithValue("@lastname", memberUpdate.LastName);
                command.Parameters.AddWithValue("@dob", memberUpdate.DOB);
                command.Parameters.AddWithValue("@contactnumber", memberUpdate.ContactNumber);
                command.Parameters.AddWithValue("@email", memberUpdate.Email);
                command.Parameters.AddWithValue("@Address", memberUpdate.Address);
                command.Parameters.AddWithValue("@age", memberUpdate.Age);
                command.Parameters.AddWithValue("@gender", memberUpdate.Gender);
                command.Parameters.AddWithValue("@height", memberUpdate.Height);
                command.Parameters.AddWithValue("@weight", memberUpdate.Weight);
                command.Parameters.AddWithValue("@membershiptype", memberUpdate.Membershiptype);
                command.Parameters.AddWithValue("@newFees", memberUpdate.fee);
                command.Parameters.AddWithValue("@id", memberId);
                command.ExecuteNonQuery();
            }
        }
        public string UpdateMemberPassword(int memberId, PasswordUpdateDto updatereq)
        {
            var data = GetmemberById(memberId);
            if (data.password== updatereq.oldPassword)
            {
                using (var connenction = new SqliteConnection(_connectionString))
                {
                    connenction.Open();
                    var command = connenction.CreateCommand();
                    command.CommandText = "UPDATE MembersDetails SET password = @newPassword;";
                    command.Parameters.AddWithValue("@newPassword", updatereq.newPassword);
                    command.ExecuteNonQuery();
                    return "Password updated";
                }
            }
            else
            {
                return "Incorrect password";
            }


        }

        public MemberResponseDTO GetmemberById(int MemberId)

        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id,Nic,FirstName,LastName,Password,DOB,ContactNumber,Email,Address,Age,Gender,Height,Weight,CreationDate,is_initalfeePaid,MembershipType,Fees FROM MembersDetails WHERE Id == @id";
                command.Parameters.AddWithValue("@id", MemberId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MemberResponseDTO()
                        {
                            Id = reader.GetInt64(0),
                            Nic = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            password = reader.GetString(4),
                            DOB = reader.GetDateTime(5),
                            ContactNumber = reader.GetString(6),
                            Email = reader.GetString(7),
                            Address = reader.GetString(8),
                            Age = reader.GetInt32(9),
                            Gender = reader.GetString(10),
                            Height = reader.GetInt32(11),
                            Weight = reader.GetInt32(12),
                            CreationDate = reader.GetDateTime(13),
                            is_initalfeePaid = reader.GetBoolean(14),
                            Membershiptype = reader.GetString(15),
                            fee = reader.GetInt32(16),

                        };
                    }
                    else
                    {
                        throw new Exception("Memeber Not Found!");
                    }
                };
            };
            return null;
        }
    }
}
