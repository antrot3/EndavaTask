using Common.Models;
using Moq;
using ServiceLayer.Service;
using ServiceLayer.Service.Interfaces;
using System.Security.Claims;

namespace ServiceLayer.Tests
{
    [TestClass]
    public class ArticleServiceTests
    {
        private Mock<IDatabaseService> _dbContextServiceMock;
        private ArticleService _articleService;

        [TestInitialize]
        public void Setup()
        {
            _dbContextServiceMock = new Mock<IDatabaseService>();
            _articleService = new ArticleService(_dbContextServiceMock.Object);
        }

        [TestMethod]
        public async Task CreateNewArticleAsync_ShouldCreateArticle_WhenUserExists()
        {
            // Arrange
            var userName = "testuser";
            var currentUser = new ApplicationUser { Id = "1", UserName = userName };
            var articleDto = new ArticleCreateDto { Title = "Test Article", Body = "Test Body" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userName) }));

            _dbContextServiceMock.Setup(x => x.GetCurrentUserAsync(userName)).ReturnsAsync(currentUser);
            _dbContextServiceMock.Setup(x => x.GetUserByIdAsync("1")).ReturnsAsync(currentUser);

            // Act
            var result = await _articleService.CreateNewArticleAsync(articleDto, claimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(articleDto.Title, result.Value.Title);
            Assert.AreEqual(currentUser.UserName, result.Value.Author);
        }

        [TestMethod]
        public async Task CreateNewArticleAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var articleDto = new ArticleCreateDto { Title = "Test Article", Body = "Test Body" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "unknownUser") }));

            _dbContextServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _articleService.CreateNewArticleAsync(articleDto, claimsPrincipal));
            Assert.AreEqual("User not found", exception.Message);
        }

        [TestMethod]
        public async Task EditArticleAsync_ShouldEditArticle_WhenArticleExists()
        {
            // Arrange
            var articleId = 1;
            var existingArticle = new Article { Id = articleId, Title = "Old Title", Body = "Old Body", UserId = "1" };
            var articleEditDto = new ArticleEditDto { ArticleId = articleId, Title = "New Title", Body = "New Body" };
            var author = new ApplicationUser { UserName = "testuser" };

            _dbContextServiceMock.Setup(x => x.GetArticleByIdAsync(articleId)).ReturnsAsync(existingArticle);
            _dbContextServiceMock.Setup(x => x.SaveChangesAsync()).Verifiable();
            _dbContextServiceMock.Setup(x => x.GetUserByIdAsync("1")).ReturnsAsync(author);

            // Act
            var result = await _articleService.EditArticleAsync(articleEditDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("New Title", result.Value.Title);
            Assert.AreEqual("New Body", result.Value.Body);
        }

        [TestMethod]
        public async Task EditArticleAsync_ShouldThrowException_WhenArticleDoesNotExist()
        {
            // Arrange
            var articleEditDto = new ArticleEditDto { ArticleId = 1, Title = "New Title", Body = "New Body" };

            _dbContextServiceMock.Setup(x => x.GetArticleByIdAsync(articleEditDto.ArticleId)).ReturnsAsync((Article)null);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await _articleService.EditArticleAsync(articleEditDto));
            Assert.AreEqual("Article not found", exception.Message);
        }

        [TestMethod]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExists()
        {
            // Arrange
            var articleId = 1;
            var article = new Article { Id = articleId, Title = "Test Title", Body = "Test Body", UserId = "1" };
            var author = new ApplicationUser { UserName = "testuser" };

            _dbContextServiceMock.Setup(x => x.GetArticleByIdAsync(articleId)).ReturnsAsync(article);
            _dbContextServiceMock.Setup(x => x.GetUserByIdAsync(article.UserId)).ReturnsAsync(author);

            // Act
            var result = await _articleService.GetArticleByIdAsync(articleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(article.Title, result.Value.Title);
            Assert.AreEqual(author.UserName, result.Value.Author);
        }

        [TestMethod]
        public async Task GetAllArticlesAsync_ShouldReturnArticles_WhenArticlesExist()
        {
            // Arrange
            var articlesFromDb = new List<Article>
            {
                new Article { Id = 1, Title = "Test Title 1", Body = "Test Body 1", UserId = "1" },
                new Article { Id = 2, Title = "Test Title 2", Body = "Test Body 2", UserId = "1" }
            };

            var authors = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", UserName = "user1" },
                new ApplicationUser { Id = "2", UserName = "user2" }
            };
            var author = new ApplicationUser { UserName = "user1" };
            var author2 = new ApplicationUser { UserName = "user2" };

            _dbContextServiceMock.Setup(x => x.GetAllArticlesAsync()).ReturnsAsync(articlesFromDb);
            _dbContextServiceMock.SetupSequence(x => x.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(author).ReturnsAsync(author2);

            // Act
            var result = await _articleService.GetAllArticlesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Value.Count);
            Assert.AreEqual("Test Title 1", result.Value[0].Title);
            Assert.AreEqual("user1", result.Value[0].Author);
            Assert.AreEqual("Test Title 2", result.Value[1].Title);
            Assert.AreEqual("user2", result.Value[1].Author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "User identity is invalid")]
        public async Task CreateNewArticleAsync_InvalidUserIdentity_ShouldThrowException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(); // Invalid user
            var articleDto = new ArticleCreateDto { Title = "Test", Body = "Test Body" };

            // Act
            await _articleService.CreateNewArticleAsync(articleDto, claimsPrincipal);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateNewArticleAsync_NullArticleDto_ShouldThrowException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "testuser")]));
            ArticleCreateDto articleDto = null;

            // Act
            await _articleService.CreateNewArticleAsync(articleDto, claimsPrincipal);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User not found")]
        public async Task CreateNewArticleAsync_UserNotFound_ShouldThrowException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "testuser")]));
            var articleDto = new ArticleCreateDto { Title = "Test", Body = "Test Body" };

            _dbContextServiceMock.Setup(db => db.GetCurrentUserAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act
            await _articleService.CreateNewArticleAsync(articleDto, claimsPrincipal);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EditArticleAsync_NullArticleEditDto_ShouldThrowException()
        {
            // Act
            await _articleService.EditArticleAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Article not found")]
        public async Task EditArticleAsync_ArticleNotFound_ShouldThrowException()
        {
            // Arrange
            var editDto = new ArticleEditDto { ArticleId = 1, Title = "Edited Title", Body = "Edited Body" };

            _dbContextServiceMock.Setup(db => db.GetArticleByIdAsync(It.IsAny<int>())).ReturnsAsync((Article)null);

            // Act
            await _articleService.EditArticleAsync(editDto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid article ID")]
        public async Task GetArticleByIdAsync_InvalidArticleId_ShouldThrowException()
        {
            // Act
            await _articleService.GetArticleByIdAsync(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Article not found")]
        public async Task GetArticleByIdAsync_ArticleNotFound_ShouldThrowException()
        {
            // Arrange
            _dbContextServiceMock.Setup(db => db.GetArticleByIdAsync(It.IsAny<int>())).ReturnsAsync((Article)null);

            // Act
            await _articleService.GetArticleByIdAsync(1);
        }
    }
}