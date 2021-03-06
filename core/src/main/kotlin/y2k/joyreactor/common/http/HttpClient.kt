package y2k.joyreactor.common.http

import org.jsoup.nodes.Document
import rx.Completable
import rx.Single
import y2k.joyreactor.common.ioSingle
import java.io.File

/**
 * Created by y2k on 5/8/16.
 */
interface HttpClient {

    fun downloadToFile(url: String, file: File): Completable

    fun downloadToFile(url: String, file: File, callback: ((Int, Int) -> Unit)?)

    fun getText(url: String): String

    fun getDocument(url: String): Document

    fun clearCookies()

    fun buildRequest(): RequestBuilder
}

interface RequestBuilder {

    fun addField(key: String, value: String): RequestBuilder

    fun putHeader(name: String, value: String): RequestBuilder

    fun get(url: String): Document

    fun post(url: String): Document
}

fun RequestBuilder.getAsync(url: String): Single<Document> {
    return ioSingle { get(url) }
}

fun HttpClient.getTextAsync(url: String): Single<String> {
    return ioSingle { getText(url) }
}