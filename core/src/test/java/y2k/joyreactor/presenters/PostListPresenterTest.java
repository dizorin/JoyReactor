package y2k.joyreactor.presenters;

import org.junit.Before;
import org.junit.Test;
import org.mockito.ArgumentCaptor;
import org.mockito.Captor;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.invocation.InvocationOnMock;
import org.mockito.stubbing.Answer;
import rx.Observable;
import y2k.joyreactor.Post;
import y2k.joyreactor.Repository;
import y2k.joyreactor.requests.PostsForTagRequest;

import java.util.*;

import static org.junit.Assert.assertEquals;
import static org.mockito.Matchers.*;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

/**
 * Created by y2k on 11/2/15.
 */
public class PostListPresenterTest {

    @Mock
    PostListPresenter.View mockView;
    @Mock
    Repository<Post> mockRepository;
    @Mock
    PostsForTagRequest mockRequest;
    @Mock
    PostsForTagRequest.Factory mockFactory;

    @Captor
    ArgumentCaptor<List<Post>> captor;

    @Before
    public void setUp() throws Exception {
        MockitoAnnotations.initMocks(this);

        when(mockRepository.queryAsync()).thenAnswer(s -> Observable.just(new ArrayList<>()));
        when(mockRepository.replaceAllAsync(any())).thenAnswer(s -> {
            List<Post> arg = (List<Post>) s.getArguments()[0];
            when(mockRepository.queryAsync()).thenAnswer(s2 -> Observable.just(new ArrayList<>(arg)));
            return Observable.just(null);
        });

        when(mockRequest.getPosts()).thenReturn(Arrays.asList(new Post[10]));
        when(mockRequest.requestAsync()).thenReturn(Observable.just(null));

        when(mockFactory.make(anyString(), anyString())).thenReturn(mockRequest);
    }

    @Test
    public void testConstructor() throws Exception {
        new PostListPresenter(mockView, mockRepository, mockFactory);

        verify(mockView).reloadPosts(captor.capture(), isNull(Integer.class));
        assertEquals(0, captor.getValue().size());

        verify(mockView).reloadPosts(captor.capture(), eq(10));
        assertEquals(10, captor.getValue().size());
    }
}