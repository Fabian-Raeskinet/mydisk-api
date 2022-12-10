﻿using AutoMapper;
using MediatR;
using MyDisk.Infrastructure.Persistence;
using MyDisk.Services.Common.Exceptions;
using MyDisk.Services.Disks.DTOs;
using MyDisk.Services.Disks.Requests;

namespace MyDisk.Services.Disks.Commands
{
    public class AttachAuthorCommandHandler : IRequestHandler<AttachAuthorRequest, DiskResponse>
    {
        private readonly IMapper _mapper;
        public AttachAuthorCommandHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<DiskResponse> Handle(AttachAuthorRequest request, CancellationToken cancellationToken)
        {
            var author = StaticContent.AuthorData.Where(a => a.Id == request.AuthorId).FirstOrDefault();
            var disk = StaticContent.DiskData.Where(d => d.Id == request.DiskId).FirstOrDefault();

            if (author == null || disk == null)
                throw new EntityNotFoundException("no matches found");

            disk.Author = author;

            return _mapper.Map<DiskResponse>(disk);
        }
    }
}
