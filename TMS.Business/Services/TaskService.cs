﻿using System;
using System.Linq;
using TMS.Entities;
using TMS.Interfaces;
using TMS.EntitiesDTO;
using System.Collections.Generic;

namespace TMS.Business.Services
{
    public class TaskService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Task> _repository;

        public TaskService(IMapper mapper, IRepository<Task> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<TaskDTO> GetAll()
        {
            return _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>(
                _repository.GetAll());
        }

        public IEnumerable<TaskDTO> GetAll(string userId) // where i`m moderator and viewer
        {
            var result = _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>(
                _repository.Filter(i => i.Moderators.Any(j => j.UserId == userId)
                        || i.Viewers.Any(j => j.UserId == userId)));

            foreach (var it in result)
            {
                FillLabelCurrentID(it, userId);
            }
            return result;
                        
        }

        public TaskDTO GetById(int ItemId, string userId)
        {
            var item = _repository.GetItem(ItemId);
            if (item == null)
                throw new Exception("Entity wasn`t found");
            CheckAbilityForView(item, userId);

            var DTOitem =  _mapper.Map<Task, TaskDTO>(item);
            FillLabelCurrentID(DTOitem, userId);
            return DTOitem;
        }

        public TaskDTO GetForEditById(int ItemId, string userId)
        {
            var item = _repository.GetItem(ItemId);
            if (item == null)
                throw new Exception("Entity wasn`t found");
            CheckAbilityForEdit(item, userId);

            var DTOitem = _mapper.Map<Task, TaskDTO>(item);
            FillLabelCurrentID(DTOitem, userId);
            return DTOitem;
        }

        public void Create(TaskDTO item, string userId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var itemEntity = _mapper.Map<TaskDTO, Task>(item);
            MergeLabels(item, itemEntity, userId);
            _repository.Create(itemEntity);
            _repository.SaveChanges();
        }

        public void Update(TaskDTO item, string userId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
            var itemEntity = _mapper.Map<TaskDTO, Task>(item);
            itemEntity.CurrentUserId = userId;
            CheckAbilityForEdit(itemEntity, userId);

            MergeLabels(item, itemEntity, userId);
         
            _repository.Update(itemEntity);
            _repository.SaveChanges();
        }

        public void Delete(int itemId, string userId)
        {
             GetForEditById(itemId, userId); // will check ability to edit
            _repository.Delete(itemId);
            _repository.SaveChanges();
        }

        internal void FillLabelCurrentID(TaskDTO dTOitem, string userId)
        {
            var TaskLabels = dTOitem.Labels.FirstOrDefault(i => i.UserId == userId);
            if (TaskLabels != null)
            {
                dTOitem.CurrentLabelID = dTOitem.Labels.FirstOrDefault(i => i.UserId == userId).LabelId;
            }
            else
            {
                dTOitem.CurrentLabelID = -1;
            }
        }

        private void MergeLabels(TaskDTO item, Task itemEntity, string userId)
        {
            if (item.CurrentLabelID != -1)
            {
                itemEntity.Labels.Add(new Task_Label_User { LabelId = item.CurrentLabelID, UserId = userId });
            } 
        }

        private void CheckAbilityForView(Task item, string userId)
        {
            if (!(item.Moderators.Any(j => j.UserId == userId) || item.Viewers.Any(j => j.UserId == userId)))
                throw new UnauthorizedAccessException("You don`t have access to this task");
        }

        private void CheckAbilityForEdit(Task item, string userId)
        {
            if (!(item.Moderators.Any(j => j.UserId == userId)))
                throw new UnauthorizedAccessException("You don`t have access to edit this task");
        }
    }
}
