﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Entities;
using TMS.EntitiesDTO;
using TMS.Interfaces;

namespace TMS.Business.Services
{
    public class TaskService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Task> _repository;
        private readonly UserService _userService;

        public TaskService(IMapper mapper, IRepository<Task> repository, UserService userService)
        {
            _mapper = mapper;
            _repository = repository;
            _userService = userService;
        }
        public IEnumerable<TaskDTO> GetAll()
        {
            return _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>(
                _repository.GetAll());
        }

        public IEnumerable<TaskDTO> GetAll(string userId) // where i`m moderator and viewer
        {
            return _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>(
                _repository.Filter(i => i.Moderators.Any(j => j.UserId == userId)
                        || i.Viewers.Any(j => j.UserId == userId)));
        }

        public IEnumerable<string> GetAllTaskModerator()
        {
            return _repository.GetAll().Select(j => j.Moderators.Select(k => k.User.Email).ToString());
        }

        public TaskDTO GetById(int ItemId)
        {
            var item = _repository.GetItem(ItemId);
            if (item == null)
                throw new Exception("Entity wasn`t found");

            return _mapper.Map<Task, TaskDTO>(item);
        }

        public void Create(TaskDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var itemEntity = _mapper.Map<TaskDTO, Task>(item);
            _repository.Create(itemEntity);
            _repository.SaveChanges();
        }

        public void Update(TaskDTO item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var itemEntity = _mapper.Map<TaskDTO, Task>(item);
            _repository.Update(itemEntity);
            _repository.SaveChanges();
        }

        public void Delete(int itemId)
        {
            _repository.Delete(itemId);
            _repository.SaveChanges();
        }
    }
}
