using ContosoUniversity.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using _ = ContosoUniversity.Shared.Features.Instructors;

namespace ContosoUniversity.Server.Infrastructure
{
    public static class InstructorExtensions
    {
        public static void Handle(this Instructor instructor, _.CreateEdit.Command message)
        {
            UpdateDetails(instructor, message);

            UpdateInstructorCourses(instructor, message.Courses);
        }

        private static void UpdateDetails(Instructor instructor, _.CreateEdit.Command message)
        {
            instructor.FirstMidName = message.FirstMidName;
            instructor.LastName = message.LastName;
            instructor.HireDate = message.HireDate.GetValueOrDefault();

            if (string.IsNullOrWhiteSpace(message.OfficeAssignmentLocation))
            {
                instructor.OfficeAssignment = null;
            }
            else if (instructor.OfficeAssignment == null)
            {
                instructor.OfficeAssignment = new OfficeAssignment
                {
                    Location = message.OfficeAssignmentLocation
                };
            }
            else
            {
                instructor.OfficeAssignment.Location = message.OfficeAssignmentLocation;
            }
        }

        private static void UpdateInstructorCourses(Instructor instructor, IEnumerable<_.CreateEdit.Command.CourseData> courses)
        {
            var assignedCourseIds = courses
                .Where(x => x.Assigned).Select(x => x.Id)
                .ToArray();

            var removedCourses = instructor.CourseAssignments
                .Where(x => !assignedCourseIds.Contains(x.CourseId))
                .ToArray();

            var previouslyAssignedCourseIds = instructor.CourseAssignments
                .Select(x => x.CourseId)
                .ToArray();

            var addedCourses = assignedCourseIds
                .Where(x => !previouslyAssignedCourseIds.Contains(x))
                .Select(x => new CourseAssignment { CourseId = x, InstructorId = instructor.Id })
                .ToArray();

            foreach (var removedCourse in removedCourses) instructor.CourseAssignments.Remove(removedCourse);

            foreach (var addedCourse in addedCourses) instructor.CourseAssignments.Add(addedCourse);
        }
    }
}
