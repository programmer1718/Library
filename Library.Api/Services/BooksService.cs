using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Contracts.Common;
using Library.Contracts.RestApi;
using Library.Contracts.DatabaseEntities;
using Library.Data.Repositories;

namespace Library.Api.Services
{
    public class BooksService : IBooksService
    {
        private readonly IMapper _mapper;
        private readonly IBooksRepository _booksRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IAuthorsRepository _authorsRepository;
        private readonly ITagsRepository _tagsRepository;

        public BooksService(
            IMapper mapper,
            IBooksRepository booksRepository,
            IPublisherRepository publisherRepository,
            IAuthorsRepository authorsRepository,
            ITagsRepository tagsRepository
        )
        {
            _mapper = mapper;
            _booksRepository = booksRepository;
            _publisherRepository = publisherRepository;
            _authorsRepository = authorsRepository;
            _tagsRepository = tagsRepository;
        }
        
        public async Task<BookDto> AddBookAsync(BookDto book)
        {
            SetPublisherIfItAlreadyExists(book);
            SetAnyAuthorsThatMayAlreadyExist(book);
            SetAnyTagsThatMayAlreadyExist(book);
            
            var bookEntity = _mapper.Map<Book>(book);
            var newBook = await _booksRepository.AddBookAsync(bookEntity);
            return _mapper.Map<BookDto>(newBook);
        }

        public async Task<BookDto> GetBookAsync(Guid id)
        {
            var bookEntity = await _booksRepository.GetBookAsync(id);
            var bookDto = _mapper.Map<BookDto>(bookEntity);
            return bookDto;
        }

        public async Task RemoveBookAsync(Guid id)
        {
            await _booksRepository.RemoveBookAsync(id);
        }

        public async Task<int> GetBooksCountAsync()
        {
            return await _booksRepository.GetBooksCountAsync();
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(int resultsPerPage, int offset, OrderBy orderBy)
        {
            var bookEntities = await _booksRepository.GetAllBooksAsync(resultsPerPage, offset, orderBy);
            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(bookEntities);
            return bookDtos;
        }

        private async void SetPublisherIfItAlreadyExists(BookDto book)
        {
            if (book.Publisher?.Name == null) return;
            var publisher = await _publisherRepository.GetPublisherByNameAsync(book.Publisher.Name);
            if (publisher == null) return;
            book.Publisher = _mapper.Map<PublisherDto>(publisher);
        }

        private async void SetAnyAuthorsThatMayAlreadyExist(BookDto book)
        {
            if (!book.Authors.Any()) return;
            
            var existingAuthors = new List<Author>();
            var nonExistingAuthors = new List<AuthorDto>();

            foreach (var incomingAuthor in book.Authors)
            {
                var author = await _authorsRepository.GetAuthorByNameAsync(incomingAuthor.FirstName, incomingAuthor.LastName);
                if (author != null)
                {
                    existingAuthors.Add(author);
                }
                else
                {
                    nonExistingAuthors.Add(incomingAuthor);
                }
            }

            if (!existingAuthors.Any()) return;
            var existingAuthorsDtos = _mapper.Map<List<AuthorDto>>(existingAuthors);
            book.Authors = existingAuthorsDtos.Concat(nonExistingAuthors);
        }

        private async void SetAnyTagsThatMayAlreadyExist(BookDto book)
        {
            if (!book.Tags.Any()) return;
            var existingTags = new List<Tag>();
            var nonExistingTags = new List<TagDto>();
            
            foreach (var incomingTag in book.Tags)
            {
                var tag = await _tagsRepository.GetTagByNameAsync(incomingTag.Name);
                if (tag != null)
                {
                    existingTags.Add(tag);
                }
                else
                {
                    nonExistingTags.Add(incomingTag);
                }
            }

            if (!existingTags.Any()) return;
            var existingTagsDtos = _mapper.Map<List<TagDto>>(existingTags);
            book.Tags = existingTagsDtos.Concat(nonExistingTags);
        }
    }
}